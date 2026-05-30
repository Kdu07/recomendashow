using System;
using BackEnd.Aventura.Domain;
using BackEnd.Aventura.Exceptions;
using BackEnd.Aventura.Services;
using BackEnd.Aventura.Tests.Fakes;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class WishlistServiceTests
    {
        private readonly FakeWishlistRepository _wishlistRepo;
        private readonly FakeEspetaculoRepository _espetaculoRepo;
        private readonly WishlistService _service;

        public WishlistServiceTests()
        {
            _wishlistRepo = new FakeWishlistRepository();
            _espetaculoRepo = new FakeEspetaculoRepository();
            _service = new WishlistService(_wishlistRepo, _espetaculoRepo);
        }

        [Fact]
        public void GetWishlist_EmptyUsuarioId_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _service.GetWishlist(Guid.Empty));
        }

        [Fact]
        public void GetWishlist_ValidUser_ShouldReturnEspetaculos()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var espetaculo1 = new Espetaculo(Guid.NewGuid(), "Show A", "Rock", "Show", "Desc", "link", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 50m);
            var espetaculo2 = new Espetaculo(Guid.NewGuid(), "Show B", "Pop", "Show", "Desc", "link", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 60m);

            _espetaculoRepo.AddEspetaculo(espetaculo1);
            _espetaculoRepo.AddEspetaculo(espetaculo2);

            _wishlistRepo.AddItem(userId, espetaculo1.Id);
            _wishlistRepo.AddItem(userId, espetaculo2.Id);

            // Act
            var result = _service.GetWishlist(userId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, e => e.Id == espetaculo1.Id);
            Assert.Contains(result, e => e.Id == espetaculo2.Id);
        }

        [Fact]
        public void Add_EmptyGuids_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _service.Add(Guid.Empty, Guid.NewGuid()));
            Assert.Throws<ArgumentException>(() => _service.Add(Guid.NewGuid(), Guid.Empty));
        }

        [Fact]
        public void Add_NonExistentEspetaculo_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var espetaculoId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<NotFoundException>(() => _service.Add(userId, espetaculoId));
        }

        [Fact]
        public void Add_AlreadyInWishlist_ShouldThrowConflictException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var espetaculo = new Espetaculo(Guid.NewGuid(), "Show A", "Rock", "Show", "Desc", "link", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 50m);
            _espetaculoRepo.AddEspetaculo(espetaculo);
            _wishlistRepo.AddItem(userId, espetaculo.Id);

            // Act & Assert
            Assert.Throws<ConflictException>(() => _service.Add(userId, espetaculo.Id));
        }

        [Fact]
        public void Add_Valid_ShouldAddToWishlist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var espetaculo = new Espetaculo(Guid.NewGuid(), "Show A", "Rock", "Show", "Desc", "link", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 50m);
            _espetaculoRepo.AddEspetaculo(espetaculo);

            // Act
            _service.Add(userId, espetaculo.Id);

            // Assert
            Assert.True(_wishlistRepo.ContainsItem(userId, espetaculo.Id));
        }

        [Fact]
        public void Remove_EmptyGuids_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _service.Remove(Guid.Empty, Guid.NewGuid()));
            Assert.Throws<ArgumentException>(() => _service.Remove(Guid.NewGuid(), Guid.Empty));
        }

        [Fact]
        public void Remove_NonExistent_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var espetaculoId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<NotFoundException>(() => _service.Remove(userId, espetaculoId));
        }

        [Fact]
        public void Remove_Valid_ShouldRemoveFromWishlist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var espetaculoId = Guid.NewGuid();
            _wishlistRepo.AddItem(userId, espetaculoId);

            // Act
            _service.Remove(userId, espetaculoId);

            // Assert
            Assert.False(_wishlistRepo.ContainsItem(userId, espetaculoId));
        }

        [Fact]
        public void Contains_EmptyGuids_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _service.Contains(Guid.Empty, Guid.NewGuid()));
            Assert.Throws<ArgumentException>(() => _service.Contains(Guid.NewGuid(), Guid.Empty));
        }

        [Fact]
        public void Contains_ChecksRepository()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var espetaculoId = Guid.NewGuid();

            // Assert
            Assert.False(_service.Contains(userId, espetaculoId));

            _wishlistRepo.AddItem(userId, espetaculoId);
            Assert.True(_service.Contains(userId, espetaculoId));
        }
    }
}
