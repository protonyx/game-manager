import {Injectable} from '@angular/core';
import {HubConnection, HubConnectionBuilder, LogLevel} from '@microsoft/signalr';
import {environment} from '../../environments/environment';

@Injectable({
    providedIn: 'root'
})
export class SignalrService {

    connection?: HubConnection

    public async connect(gameId: string, accessToken: string){
        const connection = new HubConnectionBuilder()
            .configureLogging(LogLevel.Information)
            .withAutomaticReconnect()
            .withUrl(environment.baseUrl + '/hubs/game', {
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

    public async endTurn(gameId: string, playerId: string) {
        if (this.connection)
            await this.connection.invoke("EndTurn", gameId, playerId)
    }
}
