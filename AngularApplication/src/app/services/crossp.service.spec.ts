import { TestBed } from '@angular/core/testing';

import { CrosspService } from './crossp.service';

describe('CrosspService', () => {
  let service: CrosspService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CrosspService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
