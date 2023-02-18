import {Injectable} from '@angular/core';
import {HubConnectionBuilder, LogLevel} from '@microsoft/signalr';
import {environment} from '../environment';

@Injectable({
    providedIn: 'root'
})
export class SignalrService {

    constructor() {

    }

    public connectToHub(accessToken: string){
        const connection = new HubConnectionBuilder()
            .configureLogging(LogLevel.Information)
            .withUrl(environment.baseUrl + 'hubs/game', {
                accessTokenFactory: () => accessToken
            })
            .build();

        connection.on("BroadcastMessage", () => {
            console.log()
        })
    }
}
