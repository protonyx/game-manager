import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

interface InstallPromptEvent extends Event {
  prompt(): Promise<void>;
  userChoice: Promise<{ outcome: 'accepted' | 'dismissed'; platform: string }>;
}

@Injectable({ providedIn: 'root' })
export class InstallPromptService {
  private deferredPrompt: InstallPromptEvent | null = null;
  private readonly available = new BehaviorSubject(false);
  readonly available$ = this.available.asObservable();

  constructor() {
    if (typeof window === 'undefined') return;

    window.addEventListener('beforeinstallprompt', (event: Event) => {
      event.preventDefault();
      this.deferredPrompt = event as InstallPromptEvent;
      this.available.next(true);
    });
    window.addEventListener('appinstalled', () => {
      this.deferredPrompt = null;
      this.available.next(false);
    });
  }

  async promptInstall(): Promise<void> {
    const prompt = this.deferredPrompt;
    if (!prompt) return;

    await prompt.prompt();
    await prompt.userChoice;
    this.deferredPrompt = null;
    this.available.next(false);
  }
}
