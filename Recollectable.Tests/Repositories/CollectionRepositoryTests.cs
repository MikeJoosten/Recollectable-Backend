using Recollectable.Core.Entities.Collections;
using Recollectable.Core.Specifications.Collections;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Recollectable.Tests.Repositories
{
    public class CollectionRepositoryTests : RecollectableTestBase
    {
        [Fact]
        public async Task GetAll_ReturnsAllCollections()
        {
            //Act
            var result = await _unitOfWork.Collections.GetAll();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(6, result.Count());
        }

        [Fact]
        public async Task GetSingle_ReturnsCollection()
        {
            //Act
            var result = await _unitOfWork.Collections.GetSingle();

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Add_AddsNewCollection()
        {
            //Arrange
            Guid id = new Guid("2cb67024-729e-4d76-bbe4-e80f929557ab");
            Collection newCollection = new Collection
            {
                Id = id,
                Type = "Banknote"
            };

            //Act
            await _unitOfWork.Collections.Add(newCollection);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(7, (await _unitOfWork.Collections.GetAll()).Count());
            Assert.Equal("Banknote", (await _unitOfWork.Collections.GetSingle(new CollectionById(id))).Type);
        }

        [Fact]
        public async Task Delete_RemovesCollectionFromDatabase()
        {
            //Arrange
            Guid id = new Guid("03a6907d-4e93-4863-bdaf-1d05140dec12");
            Collection collection = await _unitOfWork.Collections.GetSingle(new CollectionById(id));

            //Act
            _unitOfWork.Collections.Delete(collection);
            await _unitOfWork.Save();

            //Assert
            Assert.Equal(5, (await _unitOfWork.Collections.GetAll()).Count());
            Assert.Null(await _unitOfWork.Collections.GetSingle(new CollectionById(id)));
        }
    }
}