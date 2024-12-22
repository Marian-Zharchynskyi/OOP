using Application.Abstraction.Interfaces;
using Application.Implementation;
using NSubstitute;
using Xunit;

namespace Api.Tests.Integration
{
    public class OrderObserverTests
    {
        [Fact]
        public void Update_ShouldReceiveMessage_WhenNotifierIsNotified()
        {
            // Arrange
            var observer = Substitute.For<IObserver>();
            var logger = Substitute.For<ILogger>();
            var notifier = new OrderNotifier(logger);
            notifier.Attach(observer);

            var message = "Order status changed";

            // Act
            notifier.Notify(message);

            // Assert
            observer.Received(1).Update(message);
            logger.Received(1).Log("Notification sent: Order status changed"); 
        }

        [Fact]
        public void Update_ShouldHandleMultipleUpdates()
        {
            // Arrange
            var observer = Substitute.For<IObserver>();
            var logger = Substitute.For<ILogger>(); 
            var notifier = new OrderNotifier(logger);
            notifier.Attach(observer);

            var firstMessage = "Order placed";
            var secondMessage = "Order shipped";

            // Act
            notifier.Notify(firstMessage);
            notifier.Notify(secondMessage);

            // Assert
            observer.Received(1).Update(firstMessage);
            observer.Received(1).Update(secondMessage);
            logger.Received(1).Log("Notification sent: Order placed"); 
            logger.Received(1).Log("Notification sent: Order shipped"); 
        }

        [Fact]
        public void Attach_ShouldLog_WhenObserverIsAttached()
        {
            // Arrange
            var observer = Substitute.For<IObserver>();
            var logger = Substitute.For<ILogger>(); 
            var notifier = new OrderNotifier(logger);

            // Act
            notifier.Attach(observer);

            // Assert
            logger.Received(1).Log("Observer attached."); 
        }

        [Fact]
        public void Detach_ShouldLog_WhenObserverIsDetached()
        {
            // Arrange
            var observer = Substitute.For<IObserver>();
            var logger = Substitute.For<ILogger>();
            var notifier = new OrderNotifier(logger);
            notifier.Attach(observer);

            // Act
            notifier.Detach(observer);

            // Assert
            logger.Received(1).Log("Observer detached."); 
        }
    }
}
