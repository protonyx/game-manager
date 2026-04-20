import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { LobbyPlayerCardComponent } from './lobby-player-card.component';
import { Player } from '../../models/models';

const makePlayer = (overrides: Partial<Player> = {}): Player => ({
  id: 'p1',
  order: 1,
  name: 'Alice',
  color: '#D32F2F',
  state: 'Active',
  trackerValues: {},
  ...overrides,
});

describe('LobbyPlayerCardComponent', () => {
  let component: LobbyPlayerCardComponent;
  let fixture: ComponentFixture<LobbyPlayerCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LobbyPlayerCardComponent, NoopAnimationsModule],
    }).compileComponents();

    fixture = TestBed.createComponent(LobbyPlayerCardComponent);
    component = fixture.componentInstance;
    component.player = makePlayer();
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('shows player name', () => {
    const el: HTMLElement = fixture.nativeElement;
    expect(el.querySelector('.player-name')?.textContent).toContain('Alice');
  });

  it('shows avatar initial as first letter of name', () => {
    expect(component.avatarInitial).toBe('A');
  });

  it('shows "Waiting…" when not ready and not host', () => {
    component.player = makePlayer({ isReady: false });
    component.isHost = false;
    fixture.detectChanges();
    const el: HTMLElement = fixture.nativeElement;
    expect(el.querySelector('.waiting-label')).toBeTruthy();
    expect(el.querySelector('.ready-label')).toBeNull();
  });

  it('shows "Ready" when ready and not host', () => {
    component.player = makePlayer({ isReady: true });
    component.isHost = false;
    fixture.detectChanges();
    const el: HTMLElement = fixture.nativeElement;
    expect(el.querySelector('.ready-label')).toBeTruthy();
    expect(el.querySelector('.waiting-label')).toBeNull();
  });

  it('adds ready class when player is ready and not host', () => {
    component.player = makePlayer({ isReady: true });
    component.isHost = false;
    fixture.detectChanges();
    const card: HTMLElement = fixture.nativeElement.querySelector('.player-card');
    expect(card.classList).toContain('ready');
  });

  it('shows "Host" label and no ready/waiting when isHost is true', () => {
    component.player = makePlayer({ isReady: false });
    component.isHost = true;
    fixture.detectChanges();
    const el: HTMLElement = fixture.nativeElement;
    expect(el.querySelector('.host-label')).toBeTruthy();
    expect(el.querySelector('.waiting-label')).toBeNull();
    expect(el.querySelector('.ready-label')).toBeNull();
  });

  it('does not add ready class when isHost is true even if isReady is true', () => {
    component.player = makePlayer({ isReady: true });
    component.isHost = true;
    fixture.detectChanges();
    const card: HTMLElement = fixture.nativeElement.querySelector('.player-card');
    expect(card.classList).not.toContain('ready');
  });
});
