import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ExperiencesComponent } from './experiences.component';
import { ExperienceService } from '../services/experience.service';
import { DestinationService } from '../services/destination.service';
import { of } from 'rxjs';
import { RouterTestingModule } from '@angular/router/testing';

describe('ExperiencesComponent', () => {
  let component: ExperiencesComponent;
  let fixture: ComponentFixture<ExperiencesComponent>;
  let experienceServiceSpy: jasmine.SpyObj<ExperienceService>;
  let destinationServiceSpy: jasmine.SpyObj<DestinationService>;

  beforeEach(async () => {
    experienceServiceSpy = jasmine.createSpyObj('ExperienceService', [
      'getUserExperiences',
      'create',
      'delete',
    ]);
    destinationServiceSpy = jasmine.createSpyObj('DestinationService', ['getList']);

    experienceServiceSpy.getUserExperiences.and.returnValue(
      of([
        {
          id: 'exp-1',
          destinationId: 'dest-1',
          destinationName: 'Iguazú',
          userId: 'u-1',
          title: 'Visita a las Cataratas',
          description: 'Impresionante experiencia natural.',
          dateExperienced: new Date().toISOString(),
          rating: 5,
          creationTime: new Date().toISOString(),
        },
      ])
    );

    destinationServiceSpy.getList.and.returnValue(of({ items: [], totalCount: 0 }));

    await TestBed.configureTestingModule({
      imports: [ExperiencesComponent, RouterTestingModule],
      providers: [
        { provide: ExperienceService, useValue: experienceServiceSpy },
        { provide: DestinationService, useValue: destinationServiceSpy },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ExperiencesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create and load user experiences', () => {
    expect(component).toBeTruthy();
    expect(component.experiences.length).toBe(1);
    expect(component.experiences[0].title).toBe('Visita a las Cataratas');
  });

  it('should open and close create modal', () => {
    component.openModal();
    expect(component.showModal).toBeTrue();

    component.closeModal();
    expect(component.showModal).toBeFalse();
  });
});
