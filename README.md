# Microservices-based Application

This project is a microservices-based application utilizing **.NET** for backend services and **Next.js** for the client application.

## Key Features

- **Backend Services**: Built with .NET, providing multiple services for auctions, bidding, notifications, and search.
- **Client Application**: Developed using Next.js with React, Tailwind CSS, NextAuth for authentication, Zustand for state management, and SignalR for real-time notifications.
- **Communication**: Service-to-service communication via HTTP requests, RabbitMQ, and gRPC.
- **Resiliency**: Configured retry policies using Polly for fault tolerance.
- **Deployment**: Dockerize services with Docker Compose and Kubernetes configurations.
- **Testing**: Comprehensive unit and integration testing implemented with XUnit.

---

## Backend Services

### Auctions

- Exposes HTTP CRUD operations for managing auctions.
- Utilizes **PostgreSQL** as the database.
- Publishes and consumes events:
  - **Consumed**: `AuctionFinished`, `BidPlaced`, `AuctionCreated`, `Fault`
  - **Published**: `AuctionCreated`
- Implements a gRPC `AuctionService` for fetching auction details by ID.

### Biddings

- Provides HTTP CRUD operations for managing bids.
- Utilizes **MongoDB** as the database.
- Consumes `AuctionCreated` event and uses gRPC `AuctionClient` to fetch auction details by ID.
- Includes a hosted service for monitoring and checking finished auctions.

### Notifications

- Implements a **SignalR** server for real-time notifications.
- Handles messages such as `AuctionCreated`, `AuctionFinished`, and `BidPlaced`.

### Search

- Offers search and filter functionalities for auctions through an exposed API endpoint.
- Utilizes **MongoDB** as the database.
- Consumes events: `AuctionCreated`, `AuctionDeleted`, `AuctionFinished`, `AuctionUpdated`, and `BidPlaced`.

### Gateway (Microsoft YARP)

- Configures reverse proxy routes and clusters to manage system services routing.

### Identity

- Uses **Identity Server** for authentication.
- Defines resources, API scopes, and client configurations.

---

## Client Application

- Built with **React** and **Next.js** for seamless, modern front-end experience.
- Styled using **Tailwind CSS**.
- Authentication managed via **NextAuth**.
- State management with **Zustand**.
- Real-time notifications handled by **SignalR**.
