import { TestBed } from '@angular/core/testing';

import { ManageBoardMemberService } from './manage-board-member.service';

describe('ManageBoardMemberService', () => {
  let service: ManageBoardMemberService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ManageBoardMemberService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
