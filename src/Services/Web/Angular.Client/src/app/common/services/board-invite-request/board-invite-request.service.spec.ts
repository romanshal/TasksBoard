import { TestBed } from '@angular/core/testing';

import { BoardInviteRequestService } from './board-invite-request.service';

describe('BoardInviteRequestService', () => {
  let service: BoardInviteRequestService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BoardInviteRequestService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
