import { TestBed } from '@angular/core/testing';

import { AuthExternalService } from './auth-external.service';

describe('AuthExternalService', () => {
  let service: AuthExternalService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AuthExternalService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
