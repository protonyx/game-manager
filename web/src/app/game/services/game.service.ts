import { Injectable } from "@angular/core";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import {
  Game,
  JoinGame,
  NewGame,
  Player,
  PlayerCredentials,
} from "../models/models";
import { Observable } from "rxjs";
import { environment } from "../../../environments/environment";

@Injectable({
  providedIn: "root",
})
export class GameService {
  constructor(private http: HttpClient) {}

  public createGame(newGame: NewGame): Observable<Game> {
    const url = `${environment.baseUrl}/api/Games`;
    return this.http.post<Game>(url, newGame);
  }

  public joinGame(game: JoinGame): Observable<PlayerCredentials> {
    const url = `${environment.baseUrl}/api/Games/Join`;
    return this.http.post<PlayerCredentials>(url, game);
  }

  public getGame(gameId: string): Observable<Game> {
    const url = `${environment.baseUrl}/api/Games/${gameId}`;

    return this.http.get<Game>(url);
  }

  public getPlayer(playerId: string): Observable<Player> {
    const url = `${environment.baseUrl}/api/Players/${playerId}`;

    return this.http.get<Player>(url);
  }

  public getPlayers(gameId: string): Observable<Player[]> {
    const url = `${environment.baseUrl}/api/Games/${gameId}/Players`;

    return this.http.get<Player[]>(url);
  }

  public updatePlayer(playerId: string, player: Player): Observable<Player> {
    const url = `${environment.baseUrl}/api/Players/${playerId}`;

    return this.http.put<Player>(url, player);
  }

  public patchPlayer(playerId: string, ops: any[]): Observable<Player> {
    const url = `${environment.baseUrl}/api/Players/${playerId}`;

    const headers = new HttpHeaders({
      "Content-Type": "application/json-patch+json",
    });

    return this.http.patch<Player>(url, ops, {
      headers: headers,
    });
  }

  public setPlayerOrder(
    playerId: string,
    newOrder: number
  ): Observable<Player> {
    const ops = [{ op: "replace", path: "/order", value: newOrder }];

    return this.patchPlayer(playerId, ops);
  }

  public setPlayerName(playerId: string, newName: string): Observable<Player> {
    const ops = [{ op: "replace", path: "/name", value: newName }];

    return this.patchPlayer(playerId, ops);
  }

  public setPlayerTracker(
    playerId: string,
    trackerId: string,
    value: number
  ): Observable<Player> {
    const ops = [{ op: "replace", path: `/trackerValues/${trackerId}`, value }];

    return this.patchPlayer(playerId, ops);
  }

  public removePlayer(playerId: string): Observable<Object> {
    const url = `${environment.baseUrl}/api/Players/${playerId}`;

    return this.http.delete(url);
  }
}
