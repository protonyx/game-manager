import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import {
  Game,
  GameSummary,
  JoinGame,
  NewGame,
  Player,
  PlayerCredentials,
} from '../models/models';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class GameService {
  readonly version = 'v1';

  constructor(private http: HttpClient) {}

  private apiUrl(resourcePath: string): string {
    return `${environment.baseUrl}/api/${this.version}/${resourcePath}`;
  }

  public createGame(newGame: NewGame): Observable<Game> {
    const url = this.apiUrl('Games');
    return this.http.post<Game>(url, newGame);
  }

  public joinGame(game: JoinGame): Observable<PlayerCredentials> {
    const url = this.apiUrl('Games/Join');
    return this.http.post<PlayerCredentials>(url, game);
  }

  public getGame(gameId: string): Observable<Game> {
    const url = this.apiUrl(`Games/${gameId}`);

    return this.http.get<Game>(url);
  }

  public getGameSummary(gameId: string): Observable<GameSummary> {
    const url = this.apiUrl(`Games/${gameId}/Summary`);

    return this.http.get<GameSummary>(url);
  }

  public getPlayer(playerId: string): Observable<Player> {
    const url = this.apiUrl(`Players/${playerId}`);

    return this.http.get<Player>(url);
  }

  public getPlayers(gameId: string): Observable<Player[]> {
    const url = this.apiUrl(`Games/${gameId}/Players`);

    return this.http.get<Player[]>(url);
  }

  public updatePlayer(playerId: string, player: Player): Observable<Player> {
    const url = this.apiUrl(`Players/${playerId}`);

    return this.http.put<Player>(url, player);
  }

  public patchPlayer(playerId: string, ops: unknown[]): Observable<Player> {
    const url = this.apiUrl(`Players/${playerId}`);

    const headers = new HttpHeaders({
      'Content-Type': 'application/json-patch+json',
    });

    return this.http.patch<Player>(url, ops, {
      headers: headers,
    });
  }

  public setPlayerOrder(
    playerId: string,
    newOrder: number,
  ): Observable<Player> {
    const ops = [{ op: 'replace', path: '/order', value: newOrder }];

    return this.patchPlayer(playerId, ops);
  }

  public setPlayerName(playerId: string, newName: string): Observable<Player> {
    const ops = [{ op: 'replace', path: '/name', value: newName }];

    return this.patchPlayer(playerId, ops);
  }

  public setPlayerTracker(
    playerId: string,
    trackerId: string,
    value: number,
  ): Observable<Player> {
    const ops = [{ op: 'replace', path: `/trackerValues/${trackerId}`, value }];

    return this.patchPlayer(playerId, ops);
  }

  public endTurn(gameId: string): Observable<never> {
    const url = this.apiUrl(`Games/${gameId}/EndTurn`);

    return this.http.post<never>(url, null);
  }

  public reorderPlayers(gameId: string, players: Player[]): Observable<never> {
    const url = this.apiUrl(`Games/${gameId}/Reorder`);
    const body = {
      playerIds: players.map((player) => player.id),
    };

    return this.http.post<never>(url, body);
  }

  public startGame(gameId: string): Observable<never> {
    const url = this.apiUrl(`Games/${gameId}/Start`);

    return this.http.post<never>(url, null);
  }

  public endGame(gameId: string): Observable<never> {
    const url = this.apiUrl(`Games/${gameId}/End`);

    return this.http.post<never>(url, null);
  }

  public removePlayer(playerId: string): Observable<never> {
    const url = this.apiUrl(`Players/${playerId}`);

    return this.http.delete<never>(url);
  }
}
