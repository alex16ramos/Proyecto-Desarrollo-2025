import { TestBed } from '@angular/core/testing';
import { RestService } from '@abp/ng.core';
import { of } from 'rxjs';
import { ExperienceService } from './experience.service';
import { ExperienceDto } from '../models/experience.model';

describe('ExperienceService', () => {
  let service: ExperienceService;
  let restServiceSpy: jasmine.SpyObj<RestService>;

  beforeEach(() => {
    restServiceSpy = jasmine.createSpyObj('RestService', ['request']);

    TestBed.configureTestingModule({
      providers: [
        ExperienceService,
        { provide: RestService, useValue: restServiceSpy },
      ],
    });

    service = TestBed.inject(ExperienceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get experiences by destination', (done) => {
    const mockExperiences: ExperienceDto[] = [
      {
        id: '1',
        destinationId: 'dest-1',
        userId: 'u-1',
        title: 'Sendero de Montaña',
        description: 'Vistas increíbles',
        dateExperienced: new Date().toISOString(),
        rating: 5,
        creationTime: new Date().toISOString(),
      },
    ];

    restServiceSpy.request.and.returnValue(of(mockExperiences));

    service.getListByDestination('dest-1').subscribe((result) => {
      expect(result.length).toBe(1);
      expect(result[0].title).toBe('Sendero de Montaña');
      done();
    });
  });
});
