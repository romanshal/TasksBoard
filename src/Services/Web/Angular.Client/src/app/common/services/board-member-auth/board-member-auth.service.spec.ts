import { TestBed } from '@angular/core/testing';

import { BoardMemberAuthService } from './board-member-auth.service';

describe('BoardMemberAuthService', () => {
  let service: BoardMemberAuthService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BoardMemberAuthService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
