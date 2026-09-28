import { inject, Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { GameService } from './game.service';

@Injectable({ providedIn: 'root' })
export class PushSubscriptionService {
  private readonly gameService = inject(GameService);

  isSupported(): boolean {
    return (
      environment.production &&
      typeof navigator !== 'undefined' &&
      'serviceWorker' in navigator &&
      typeof window !== 'undefined' &&
      'PushManager' in window &&
      'Notification' in window
    );
  }

  permission(): NotificationPermission {
    return this.isSupported() ? Notification.permission : 'denied';
  }

  isIos(): boolean {
    return typeof navigator !== 'undefined' && /iphone|ipad|ipod/i.test(navigator.userAgent);
  }

  isStandalone(): boolean {
    return (
      (typeof window !== 'undefined' && window.matchMedia('(display-mode: standalone)').matches) ||
      (typeof navigator !== 'undefined' &&
        (navigator as Navigator & { standalone?: boolean }).standalone === true)
    );
  }

  async isSubscribed(): Promise<boolean> {
    if (!this.isSupported()) return false;
    const registration = await navigator.serviceWorker.ready;
    return (await registration.pushManager.getSubscription()) !== null;
  }

  async requestPermissionAndSubscribe(): Promise<boolean> {
    if (!this.isSupported()) return false;
    if (!environment.vapidPublicKey.trim()) {
      throw new Error('Web Push is not configured: the VAPID public key is missing.');
    }
    if ((await Notification.requestPermission()) !== 'granted') return false;

    const registration = await navigator.serviceWorker.ready;
    const subscription =
      (await registration.pushManager.getSubscription()) ??
      (await registration.pushManager.subscribe({
        userVisibleOnly: true,
        applicationServerKey: this.urlBase64ToUint8Array(environment.vapidPublicKey),
      }));
    await this.registerSubscription(subscription);
    return true;
  }

  async resubscribeExisting(): Promise<boolean> {
    if (!this.isSupported() || this.permission() !== 'granted') return false;
    const registration = await navigator.serviceWorker.ready;
    const subscription = await registration.pushManager.getSubscription();
    if (!subscription) return false;

    await this.registerSubscription(subscription);
    return true;
  }

  async unsubscribe(): Promise<void> {
    if (!this.isSupported()) return;
    const registration = await navigator.serviceWorker.ready;
    const subscription = await registration.pushManager.getSubscription();
    if (!subscription) return;

    try {
      await firstValueFrom(this.gameService.unsubscribePush(subscription.endpoint));
    } catch (error) {
      console.warn('Could not remove the push subscription from the server.', error);
    }
    await subscription.unsubscribe();
  }

  private async registerSubscription(subscription: PushSubscription): Promise<void> {
    const keys = subscription.toJSON().keys;
    const p256dh = keys?.['p256dh'];
    const auth = keys?.['auth'];
    if (!p256dh || !auth) {
      throw new Error('The browser returned a push subscription without encryption keys.');
    }

    await firstValueFrom(
      this.gameService.subscribePush({
        endpoint: subscription.endpoint,
        p256dh,
        auth,
      }),
    );
  }

  private urlBase64ToUint8Array(base64: string): Uint8Array {
    const padding = '='.repeat((4 - (base64.length % 4)) % 4);
    const value = (base64 + padding).replace(/-/g, '+').replace(/_/g, '/');
    const raw = atob(value);
    return Uint8Array.from([...raw].map((character) => character.charCodeAt(0)));
  }
}
