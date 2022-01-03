using Cadmus.Cli.Core;
using Cadmus.Core;
using Cadmus.Core.Config;
using Cadmus.Core.Storage;
using Cadmus.General.Parts;
using Cadmus.Mongo;
using Cadmus.Philology.Parts;
using Fusi.Tools.Config;
using System;
using System.Reflection;

namespace Cadmus.Cli.Plugin.Mqdq
{
    [Tag("repository-factory-provider.mqdq")]
    public sealed class MqdqCliRepositoryFactoryProvider :
        ICliRepositoryFactoryProvider
    {
        private readonly TagAttributeToTypeMap _map;
        private readonly IPartTypeProvider _partTypeProvider;

        public string ConnectionString { get; set; }

        public MqdqCliRepositoryFactoryProvider()
        {
            _map = new TagAttributeToTypeMap();
            _map.Add(new[]
            {
                // Cadmus.General.Parts
                typeof(NotePart).GetTypeInfo().Assembly,
                // Cadmus.Philology.Parts
                typeof(ApparatusLayerFragment).GetTypeInfo().Assembly
            });

            _partTypeProvider = new StandardPartTypeProvider(_map);
        }

        public ICadmusRepository CreateRepository(string database)
        {
            if (database == null)
                throw new ArgumentNullException(nameof(database));

            // create the repository (no need to use container here)
            MongoCadmusRepository repository =
                new(_partTypeProvider, new StandardItemSortKeyBuilder());

            repository.Configure(new MongoCadmusRepositoryOptions
            {
                ConnectionString = string.Format(ConnectionString, database)
            });

            return repository;
        }
    }
}
