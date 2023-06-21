import {ChangeDetectionStrategy, ChangeDetectorRef, Component, Input, OnDestroy, OnInit} from '@angular/core';
import {interval, Subject, takeUntil, Timestamp} from "rxjs";
import { NgIf, DecimalPipe } from '@angular/common';

@Component({
    selector: 'app-turn-timer',
    templateUrl: './turn-timer.component.html',
    styleUrls: ['./turn-timer.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush,
    standalone: true,
    imports: [NgIf, DecimalPipe]
})
export class TurnTimerComponent implements OnInit, OnDestroy {
    @Input()
    startTime: string | null | undefined;

    private destroyed$ = new Subject();

    constructor(private changeDetector: ChangeDetectorRef) {}

    ngOnInit(): void {
        interval(1000).pipe(
            takeUntil(this.destroyed$)
        ).subscribe(() => {
            this.changeDetector.detectChanges();
        })
    }

    ngOnDestroy() {
        this.destroyed$.next(null);
        this.destroyed$.complete();
    }

    getElapsedTime(): TimeSpan {
        if (this.startTime) {
            let totalSeconds = Math.floor((+new Date() - +new Date(this.startTime)) / 1000);

            let hours = 0;
            let minutes = 0;

            if (totalSeconds >= 3600) {
                hours = Math.floor(totalSeconds / 3600);
                totalSeconds -= 3600 * hours;
            }

            if (totalSeconds >= 60) {
                minutes = Math.floor(totalSeconds / 60);
                totalSeconds -= 60 * minutes;
            }

            const seconds = totalSeconds;

            return {
                hours: hours,
                minutes: minutes,
                seconds: seconds
            };
        }

        return {
            hours: 0,
            minutes: 0,
            seconds: 0
        }
    }

}

export interface TimeSpan {
    hours: number;
    minutes: number;
    seconds: number;
}
