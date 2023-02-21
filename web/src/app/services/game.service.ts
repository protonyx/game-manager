import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Game, JoinGame, NewGame, PlayerCredentials} from "../models/models";
import {Observable} from "rxjs";
import {environment} from "../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class GameService {

  constructor(private http: HttpClient) { }

  public createGame(newGame: NewGame): Observable<Game> {
    const url = environment.baseUrl + '/api/Games'
    return this.http.post<Game>(url, newGame)
  }

  public joinGame(game: JoinGame): Observable<PlayerCredentials> {
    const url = environment.baseUrl + '/api/Games/Join'
    return this.http.post<PlayerCredentials>(url, game)
  }
}
