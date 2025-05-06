import { TestBed } from '@angular/core/testing';

import { BoardMessageService } from './board-message.service';

describe('BoardMessageService', () => {
  let service: BoardMessageService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BoardMessageService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
