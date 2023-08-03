import { Injectable } from '@angular/core';
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { Store } from '@ngrx/store';
import { GameHubActions } from '../state/game.actions';
import {
  GameStateChangedMessage,
  PlayerJoinedMessage,
  PlayerLeftMessage,
  PlayerStateChangedMessage,
} from '../models/messages';
import { Subject, Subscription, takeUntil, timer } from 'rxjs';
import { PlayerCredentials } from '../models/models';

@Injectable({
  providedIn: 'root',
})
export class GameHubService {
  connection?: HubConnection;

  close$: Subject<void> = new Subject<void>();

  heartbeat$: Subscription | undefined;

  constructor(private store: Store) {}

  public async connect(gameId: string, accessToken: string) {
    if (this.connection) {
      await this.disconnect();
    }

    const connection = new HubConnectionBuilder()
      .configureLogging(LogLevel.Information)
      .withAutomaticReconnect()
      .withUrl(environment.baseUrl + '/hubs/game', {
        accessTokenFactory: () => accessToken,
      })
      .build();

    connection.on('GameStateChanged', (data: GameStateChangedMessage) => {
      this.store.dispatch(GameHubActions.gameUpdated(data));
    });
    connection.on('PlayerJoined', (data: PlayerJoinedMessage) => {
      this.store.dispatch(GameHubActions.playerJoined(data));
    });
    connection.on('PlayerStateChanged', (data: PlayerStateChangedMessage) => {
      this.store.dispatch(GameHubActions.playerUpdated(data));
    });
    connection.on('PlayerLeft', (data: PlayerLeftMessage) => {
      this.store.dispatch(GameHubActions.playerLeft(data));
    });
    connection.on('UpdateCredentials', (data: PlayerCredentials) => {
      this.store.dispatch(
        GameHubActions.credentialsUpdated({ credentials: data })
      );
    });
    connection.onclose(async () => {
      console.log('SignalR disconnected');
      this.store.dispatch(GameHubActions.hubDisconnected());
      this.close$.next();
    });
    connection.onreconnected(() => {
      console.log('SignalR reconnected');
      this.store.dispatch(GameHubActions.hubConnected());
      this.setupHeartbeat();
    });

    try {
      await connection.start();
      console.log('SignalR connected');
      this.store.dispatch(GameHubActions.hubConnected());
      this.setupHeartbeat();

      this.connection = connection;
    } catch (err) {
      console.error(err);
    }
  }

  public async reconnect() {
    if (
      this.connection &&
      this.connection.state == HubConnectionState.Disconnected
    ) {
      await this.connection.start();
      this.store.dispatch(GameHubActions.hubConnected());
      this.setupHeartbeat();
    }
  }

  public async disconnect() {
    if (
      this.connection &&
      this.connection.state == HubConnectionState.Connected
    ) {
      await this.connection.stop();
      this.close$.next();

      this.connection = undefined;
    }
  }

  public async heartbeat() {
    if (
      this.connection &&
      this.connection.state == HubConnectionState.Connected
    )
      await this.connection.invoke('Heartbeat');
  }

  private setupHeartbeat(): void {
    this.heartbeat$ = timer(5000, 30000)
      .pipe(takeUntil(this.close$))
      .subscribe({
        next: () => this.heartbeat(),
        complete: () => console.log('Heartbeat stopped'),
      });
  }
}
