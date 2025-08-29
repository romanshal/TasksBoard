import { TestBed } from '@angular/core/testing';

import { AuthTokenTimerService } from './auth-token-timer.service';

describe('AuthTokenTimerService', () => {
  let service: AuthTokenTimerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AuthTokenTimerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
