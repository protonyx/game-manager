import {Injectable} from '@angular/core';
import {HubConnection, HubConnectionBuilder, LogLevel} from '@microsoft/signalr';
import {environment} from '../../../environments/environment';
import {Store} from "@ngrx/store";
import {GameActions, GameHubActions} from "../state/game.actions";
import {
    GameStateChangedMessage,
    PlayerJoinedMessage,
    PlayerLeftMessage,
    PlayerStateChangedMessage
} from "../models/messages";

@Injectable({
    providedIn: 'root'
})
export class SignalrService {

    connection?: HubConnection

    constructor(private store: Store){}

    public async connect(gameId: string, accessToken: string){
        const connection = new HubConnectionBuilder()
            .configureLogging(LogLevel.Information)
            .withAutomaticReconnect()
            .withUrl(environment.baseUrl + '/hubs/game', {
                accessTokenFactory: () => accessToken
            })
            .build();

        connection.on("GameStateChanged", (data: GameStateChangedMessage) => {
            console.log("SignalR - GameStateChanged", data);
            this.store.dispatch(GameHubActions.gameUpdated(data))
        });
        connection.on("PlayerJoined", (data: PlayerJoinedMessage) => {
            console.log("SignalR - PlayerJoined", data);
            this.store.dispatch(GameHubActions.playerJoined(data))
        });
        connection.on("PlayerStateChanged", (data: PlayerStateChangedMessage) => {
            console.log("SignalR - PlayerStateChanged", data);
            this.store.dispatch(GameHubActions.playerUpdated(data))
        });
        connection.on("PlayerLeft", (data: PlayerLeftMessage) => {
            console.log("SignalR - PlayerLeft", data);
            this.store.dispatch(GameHubActions.playerLeft(data))
        })
        connection.onclose(async () => {
            //await start()
        });

        try {
            await connection.start()
            console.log("SignalR connected")

            this.connection = connection
        } catch (err) {
            console.log(err)
            // setTimeout(start, 5000)
        }
    }

    public async disconnect() {
        if (this.connection)
            await this.connection.stop()
    }

    public async heartbeat() {
        if (this.connection)
            await this.connection.invoke("Heartbeat")
    }

    public async endTurn() {
        if (this.connection)
            await this.connection.invoke("EndTurn")
    }
}
