import { Injectable } from '@angular/core';
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {Game, JoinGame, NewGame, Player, PlayerCredentials} from "../models/models";
import {Observable} from "rxjs";
import {environment} from "../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class GameService {

  constructor(private http: HttpClient) { }

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
}
