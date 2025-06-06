import { TestBed } from '@angular/core/testing';

import { ManageBoardInviteRequestService } from './manage-board-invite-request.service';

describe('ManageBoardInviteRequestService', () => {
  let service: ManageBoardInviteRequestService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ManageBoardInviteRequestService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
