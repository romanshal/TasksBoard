import { TestBed } from '@angular/core/testing';

import { SearchMemberService } from './search-member.service';

describe('SearchMemberService', () => {
  let service: SearchMemberService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SearchMemberService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
