import { TestBed } from '@angular/core/testing';

import { BoardNoticeService } from './board-notice.service';

describe('BoardNoticeService', () => {
  let service: BoardNoticeService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BoardNoticeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
