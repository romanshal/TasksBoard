import { TestBed } from '@angular/core/testing';

import { BoardPermissionService } from './board-permission.service';

describe('BoardPermissionService', () => {
  let service: BoardPermissionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BoardPermissionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
