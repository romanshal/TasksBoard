import { TestBed } from '@angular/core/testing';

import { ManageBoardAccessRequestService } from './manage-board-access-request.service';

describe('ManageBoardAccessRequestService', () => {
  let service: ManageBoardAccessRequestService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ManageBoardAccessRequestService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
