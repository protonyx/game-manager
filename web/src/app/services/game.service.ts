import { Injectable } from '@angular/core';
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {Game, JoinGame, NewGame, Player, PlayerCredentials} from "../models/models";
import {Observable} from "rxjs";
import {environment} from "../../environments/environment";

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

  public getGame(gameId: string, token: string): Observable<Game> {
    const url = `${environment.baseUrl}/api/Games/${gameId}`;
    const httpOptions = {
      headers: new HttpHeaders({
        Authorization: `Bearer ${token}`
      })
    }
    return this.http.get<Game>(url, httpOptions);
  }

  public getPlayers(gameId: string, token: string): Observable<Player[]> {
    const url = `${environment.baseUrl}/api/Games/${gameId}/Players`;
    const httpOptions = {
      headers: new HttpHeaders({
        Authorization: `Bearer ${token}`
      })
    }
    return this.http.get<Player[]>(url, httpOptions);
  }
}
