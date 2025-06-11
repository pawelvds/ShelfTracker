# 📚 ShelfTracker

A modern book management system with automatic change history tracking, built with ASP.NET Core and featuring a beautiful, responsive web interface.

![ShelfTracker Demo](https://img.shields.io/badge/Demo-Live-brightgreen) ![.NET](https://img.shields.io/badge/.NET-9.0-blue) ![SQLite](https://img.shields.io/badge/Database-SQLite-lightblue)

## 🚀 Quick Start

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Any modern web browser

### Installation & Running

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd ShelfTracker
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

4. **Access the application**
   - **Web Interface**: http://localhost:5000
   - **API Documentation**: http://localhost:5000/swagger

### First-Time Setup

The application includes seed data for demonstration:
- Sample books with authors and descriptions
- Ready-to-use filters and grouping examples

## 📊 API Endpoints

### Books Management
- `GET /api/Books` - List all books
- `GET /api/Books/{id}` - Get specific book
- `POST /api/Books` - Create new book
- `PUT /api/Books/{id}` - Update book
- `DELETE /api/Books/{id}` - Soft delete book

### Change History
- `GET /api/Changes` - Get change history with filtering and pagination
- `GET /api/Changes/book/{bookId}` - Get changes for specific book

### Query Parameters
- `?page=1&pageSize=25` - Pagination
- `?changeType=TitleChanged` - Filter by change type
- `?bookId=123` - Filter by book
- `?fromDate=2024-01-01&toDate=2024-12-31` - Date range filtering
- `?groupBy=day` - Group results (day, week, month, changetype)
- `?sortBy=ChangedAt&sortDirection=desc` - Sorting options
