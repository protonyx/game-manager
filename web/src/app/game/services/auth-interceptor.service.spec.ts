import { TestBed } from '@angular/core/testing';
import { HTTP_INTERCEPTORS, HttpClient } from '@angular/common/http';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideMockStore, MockStore } from '@ngrx/store/testing';

import { AuthInterceptorService } from './auth-interceptor.service';
import { GamesApiActions } from '../state/game.actions';

const TEST_URL = '/api/v1/test';

describe('AuthInterceptorService', () => {
  let service: AuthInterceptorService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        AuthInterceptorService,
        provideMockStore(),
      ]
    });
    service = TestBed.inject(AuthInterceptorService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('intercept', () => {
    let httpClient: HttpClient;
    let httpMock: HttpTestingController;
    let store: MockStore;

    describe('with credentials in store', () => {
      beforeEach(() => {
        TestBed.configureTestingModule({
          providers: [
            provideHttpClient(withInterceptorsFromDi()),
            provideHttpClientTesting(),
            provideMockStore({
              initialState: {
                game: {
                  credentials: { token: 'test-token', gameId: 'g1', playerId: 'p1', isHost: false }
                }
              }
            }),
            { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptorService, multi: true },
          ]
        });
        httpClient = TestBed.inject(HttpClient);
        httpMock = TestBed.inject(HttpTestingController);
        store = TestBed.inject(MockStore);
      });

      afterEach(() => httpMock.verify());

      it('adds Authorization Bearer header when credentials are present', () => {
        httpClient.get(TEST_URL).subscribe();
        const req = httpMock.expectOne(TEST_URL);
        expect(req.request.headers.get('Authorization')).toBe('Bearer test-token');
        req.flush({});
      });

      it('dispatches authenticationError when a 401 response is received', () => {
        const dispatchSpy = spyOn(store, 'dispatch');
        httpClient.get(TEST_URL).subscribe({ error: () => {} });
        const req = httpMock.expectOne(TEST_URL);
        req.flush('Unauthorized', { status: 401, statusText: 'Unauthorized' });
        expect(dispatchSpy).toHaveBeenCalledWith(GamesApiActions.authenticationError());
      });
    });

    describe('without credentials in store', () => {
      beforeEach(() => {
        TestBed.configureTestingModule({
          providers: [
            provideHttpClient(withInterceptorsFromDi()),
            provideHttpClientTesting(),
            provideMockStore({ initialState: { game: { credentials: null } } }),
            { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptorService, multi: true },
          ]
        });
        httpClient = TestBed.inject(HttpClient);
        httpMock = TestBed.inject(HttpTestingController);
      });

      afterEach(() => httpMock.verify());

      it('passes request without Authorization header when no credentials', () => {
        httpClient.get(TEST_URL).subscribe();
        const req = httpMock.expectOne(TEST_URL);
        expect(req.request.headers.has('Authorization')).toBeFalse();
        req.flush({});
      });
    });
  });
});
