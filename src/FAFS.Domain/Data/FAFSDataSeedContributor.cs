using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FAFS.Destinations;
using FAFS.Experiences;
using FAFS.Favorites;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace FAFS.Data
{
    public class FAFSDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IGuidGenerator _guidGenerator;

        public FAFSDataSeedContributor(
            IServiceProvider serviceProvider,
            IGuidGenerator guidGenerator)
        {
            _serviceProvider = serviceProvider;
            _guidGenerator = guidGenerator;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            var destinationRepository = _serviceProvider.GetService(typeof(IRepository<Destination, Guid>)) as IRepository<Destination, Guid>;
            var experienceRepository = _serviceProvider.GetService(typeof(IRepository<Experience, Guid>)) as IRepository<Experience, Guid>;
            var ratingRepository = _serviceProvider.GetService(typeof(IRepository<DestinationRating, Guid>)) as IRepository<DestinationRating, Guid>;

            if (destinationRepository == null || experienceRepository == null || ratingRepository == null)
            {
                return;
            }

            if (await destinationRepository.GetCountAsync() > 0)
            {
                return;
            }

            var cataratasId = _guidGenerator.Create();
            var barilocheId = _guidGenerator.Create();
            var machuPicchuId = _guidGenerator.Create();
            var parisId = _guidGenerator.Create();
            var tokioId = _guidGenerator.Create();

            var destinations = new List<Destination>
            {
                new Destination(
                    cataratasId,
                    "Cataratas del Iguazú",
                    "Argentina",
                    "Puerto Iguazú",
                    "https://images.unsplash.com/photo-1589556264800-08ae9e129a8c?auto=format&fit=crop&w=800&q=80",
                    DateTime.UtcNow,
                    new Coordinates("-25.6953", "-54.4367"),
                    "Una de las 7 Maravillas Naturales del Mundo, impresionantes saltos de agua rodeados de selva subtropical."
                ),
                new Destination(
                    barilocheId,
                    "San Carlos de Bariloche",
                    "Argentina",
                    "Bariloche",
                    "https://images.unsplash.com/photo-1544644181-1484b3fdfc62?auto=format&fit=crop&w=800&q=80",
                    DateTime.UtcNow,
                    new Coordinates("-41.1343", "-71.3090"),
                    "Capital nacional del chocolate y turismo aventura en la Patagonia frente al lago Nahuel Huapi."
                ),
                new Destination(
                    machuPicchuId,
                    "Machu Picchu",
                    "Perú",
                    "Cusco",
                    "https://images.unsplash.com/photo-1526392060635-9d6019884377?auto=format&fit=crop&w=800&q=80",
                    DateTime.UtcNow,
                    new Coordinates("-13.1631", "-72.5450"),
                    "Antigua ciudadela inca enclavada en lo alto de los Andes peruanos."
                ),
                new Destination(
                    parisId,
                    "Torre Eiffel y Centro Histórico",
                    "Francia",
                    "París",
                    "https://images.unsplash.com/photo-1511739001486-6bfe10ce785f?auto=format&fit=crop&w=800&q=80",
                    DateTime.UtcNow,
                    new Coordinates("48.8584", "2.2945"),
                    "La Ciudad de la Luz, con su emblemática arquitectura, gastronomía de primer nivel y museos universales."
                ),
                new Destination(
                    tokioId,
                    "Shibuya y Shinjuku",
                    "Japón",
                    "Tokio",
                    "https://images.unsplash.com/photo-1503899036084-c55cdd92da26?auto=format&fit=crop&w=800&q=80",
                    DateTime.UtcNow,
                    new Coordinates("35.6762", "139.6503"),
                    "Fascinante mezcla entre rascacielos ultramodernos, tecnología y templos históricos."
                )
            };

            foreach (var dest in destinations)
            {
                await destinationRepository.InsertAsync(dest, autoSave: true);
            }

            // Seed initial Experiences
            var dummyUserId = _guidGenerator.Create();
            var experiences = new List<Experience>
            {
                new Experience(
                    _guidGenerator.Create(),
                    cataratasId,
                    dummyUserId,
                    "Paseo náutico bajo la Garganta del Diablo",
                    "Una experiencia inolvidable. La fuerza del agua te deja sin palabras.",
                    DateTime.UtcNow.AddDays(-10),
                    5,
                    "Recomiendo llevar ropa impermeable."
                ),
                new Experience(
                    _guidGenerator.Create(),
                    barilocheId,
                    dummyUserId,
                    "Circuito Chico en otoño",
                    "Los colores de los árboles reflejados en los lagos son espectaculares.",
                    DateTime.UtcNow.AddDays(-20),
                    5,
                    "No dejen de probar los chocolates artesanales."
                )
            };

            foreach (var exp in experiences)
            {
                await experienceRepository.InsertAsync(exp, autoSave: true);
            }

            // Seed initial Ratings
            var ratings = new List<DestinationRating>
            {
                new DestinationRating(_guidGenerator.Create(), dummyUserId, cataratasId, 5, "Excelente destino"),
                new DestinationRating(_guidGenerator.Create(), dummyUserId, barilocheId, 5, "Mágico lugar"),
                new DestinationRating(_guidGenerator.Create(), dummyUserId, machuPicchuId, 5, "Impresionante historia"),
                new DestinationRating(_guidGenerator.Create(), dummyUserId, parisId, 4, "Muy romántico y hermoso"),
                new DestinationRating(_guidGenerator.Create(), dummyUserId, tokioId, 5, "Increíble cultura y seguridad")
            };

            foreach (var r in ratings)
            {
                await ratingRepository.InsertAsync(r, autoSave: true);
            }
        }
    }
}
