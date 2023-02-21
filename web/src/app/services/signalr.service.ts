import {Injectable} from '@angular/core';
import {HubConnection, HubConnectionBuilder, LogLevel} from '@microsoft/signalr';
import {environment} from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class SignalrService {

    connection: HubConnection

    constructor() {

    }

    public async connect(gameId: string, accessToken: string){
        const connection = new HubConnectionBuilder()
            .configureLogging(LogLevel.Information)
            .withUrl(environment.baseUrl + 'hubs/game', {
                accessTokenFactory: () => accessToken
            })
            .build();

        connection.on("GameStateChanged", () => {
            console.log()
        })
        connection.on("PlayerJoined", () => {
            console.log()
        })
        connection.onclose(async () => {
            //await start()
        })

        try {
            await connection.start()
            console.log("SignalR connected")

            // Register
            await connection.invoke("RegisterClient", gameId);

            this.connection = connection
        } catch (err) {
            console.log(err)
            // setTimeout(start, 5000)
        }
    }

    public async disconnect() {
        await this.connection.stop()
    }

    public async endTurn(gameId: string, playerId: string) {
        await this.connection.invoke("EndTurn", gameId, playerId)
    }
}
