import { TestBed } from '@angular/core/testing';

import { BoardAccessRequestService } from './board-access-request.service';

describe('BoardAccessRequestService', () => {
  let service: BoardAccessRequestService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BoardAccessRequestService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
