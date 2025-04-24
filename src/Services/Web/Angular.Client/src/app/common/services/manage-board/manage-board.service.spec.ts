import { TestBed } from '@angular/core/testing';

import { ManageBoardService } from './manage-board.service';

describe('ManageBoardService', () => {
  let service: ManageBoardService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ManageBoardService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
