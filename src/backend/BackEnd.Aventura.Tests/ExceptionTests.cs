using System;
using BackEnd.Aventura.Exceptions;
using Xunit;

namespace BackEnd.Aventura.Tests
{
    public class ExceptionTests
    {
        [Fact]
        public void NotFoundException_ShouldSetMessageAndInheritFromException()
        {
            // Arrange
            var message = "Recurso não encontrado";

            // Act
            var ex = new NotFoundException(message);

            // Assert
            Assert.Equal(message, ex.Message);
            Assert.IsAssignableFrom<Exception>(ex);
        }

        [Fact]
        public void ConflictException_ShouldSetMessageAndInheritFromException()
        {
            // Arrange
            var message = "Conflito de recurso";

            // Act
            var ex = new ConflictException(message);

            // Assert
            Assert.Equal(message, ex.Message);
            Assert.IsAssignableFrom<Exception>(ex);
        }
    }
}
