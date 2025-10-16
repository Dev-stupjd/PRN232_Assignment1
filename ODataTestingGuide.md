# OData Testing Guide for FU News Management API

## 🚀 Quick Start

1. **Start the API:**
   ```bash
   cd ApiServer
   dotnet run
   ```

2. **Access OData endpoints:**
   - Base URL: `https://localhost:7067/odata`
   - Metadata: `https://localhost:7067/odata/$metadata`

## 📊 OData Query Examples

### **1. Basic Queries**

#### Get All Categories
```bash
GET https://localhost:7067/odata/Category
```

#### Get Category by ID
```bash
GET https://localhost:7067/odata/Category(1)
```

#### Get All News Articles
```bash
GET https://localhost:7067/odata/NewsArticle
```

#### Get Active News Articles Only
```bash
GET https://localhost:7067/odata/NewsArticle/Active
```

### **2. Filtering ($filter)**

#### Filter Categories by Name
```bash
GET https://localhost:7067/odata/Category?$filter=CategoryName eq 'Technology'
```

#### Filter News Articles by Status
```bash
GET https://localhost:7067/odata/NewsArticle?$filter=NewsStatus eq true
```

#### Filter News Articles by Date Range
```bash
GET https://localhost:7067/odata/NewsArticle?$filter=CreatedDate ge 2024-12-01T00:00:00Z and CreatedDate le 2024-12-31T23:59:59Z
```

#### Filter News Articles by Author
```bash
GET https://localhost:7067/odata/NewsArticle?$filter=CreatedById eq 2
```

#### Complex Filtering
```bash
GET https://localhost:7067/odata/NewsArticle?$filter=NewsStatus eq true and CategoryId eq 1
```

### **3. Sorting ($orderby)**

#### Sort Categories by Name
```bash
GET https://localhost:7067/odata/Category?$orderby=CategoryName
```

#### Sort News Articles by Date (Descending)
```bash
GET https://localhost:7067/odata/NewsArticle?$orderby=CreatedDate desc
```

#### Multiple Sort Criteria
```bash
GET https://localhost:7067/odata/NewsArticle?$orderby=CategoryId, CreatedDate desc
```

### **4. Field Selection ($select)**

#### Select Specific Fields
```bash
GET https://localhost:7067/odata/Category?$select=CategoryId,CategoryName
```

#### Select News Article Fields
```bash
GET https://localhost:7067/odata/NewsArticle?$select=NewsArticleId,NewsTitle,Headline,CreatedDate
```

### **5. Pagination ($top, $skip)**

#### Get First 5 Categories
```bash
GET https://localhost:7067/odata/Category?$top=5
```

#### Skip First 10, Get Next 5
```bash
GET https://localhost:7067/odata/Category?$skip=10&$top=5
```

#### Pagination with Filtering
```bash
GET https://localhost:7067/odata/NewsArticle?$filter=NewsStatus eq true&$top=10&$skip=20
```

### **6. Counting ($count)**

#### Get Count of Categories
```bash
GET https://localhost:7067/odata/Category/$count
```

#### Get Count with Filter
```bash
GET https://localhost:7067/odata/NewsArticle/$count?$filter=NewsStatus eq true
```

### **7. Expansion ($expand)**

#### Expand Category with Parent Category
```bash
GET https://localhost:7067/odata/Category?$expand=ParentCategory
```

#### Expand News Article with Category
```bash
GET https://localhost:7067/odata/NewsArticle?$expand=Category
```

#### Expand News Article with Author
```bash
GET https://localhost:7067/odata/NewsArticle?$expand=CreatedBy
```

#### Multiple Expansions
```bash
GET https://localhost:7067/odata/NewsArticle?$expand=Category,CreatedBy
```

### **8. Complex Queries**

#### Get Active News Articles with Category Info, Sorted by Date
```bash
GET https://localhost:7067/odata/NewsArticle?$filter=NewsStatus eq true&$expand=Category&$orderby=CreatedDate desc&$top=10
```

#### Get Categories with News Count
```bash
GET https://localhost:7067/odata/Category?$expand=NewsArticles&$select=CategoryId,CategoryName
```

#### Search News Articles by Title
```bash
GET https://localhost:7067/odata/NewsArticle?$filter=contains(NewsTitle,'AI')
```

## 🔧 CRUD Operations

### **Create (POST)**

#### Create New Category
```bash
POST https://localhost:7067/odata/Category
Content-Type: application/json

{
  "CategoryName": "New Technology",
  "CategoryDesciption": "Latest technology trends",
  "IsActive": true
}
```

#### Create New News Article
```bash
POST https://localhost:7067/odata/NewsArticle
Content-Type: application/json

{
  "NewsTitle": "New Tech Breakthrough",
  "Headline": "Revolutionary Technology",
  "NewsContent": "Content here...",
  "CategoryId": 1,
  "CreatedById": 2,
  "NewsStatus": true
}
```

### **Update (PUT)**

#### Update Category
```bash
PUT https://localhost:7067/odata/Category(1)
Content-Type: application/json

{
  "CategoryId": 1,
  "CategoryName": "Updated Technology",
  "CategoryDesciption": "Updated description",
  "IsActive": true
}
```

### **Delete (DELETE)**

#### Delete Category (will fail if has articles)
```bash
DELETE https://localhost:7067/odata/Category(1)
```

#### Delete News Article
```bash
DELETE https://localhost:7067/odata/NewsArticle('ART20241201001')
```

## 🧪 Testing Business Rules

### **1. Account Deletion Rule**
```bash
# Try to delete account that has articles (should fail)
DELETE https://localhost:7067/odata/SystemAccount(2)
```

### **2. Category Deletion Rule**
```bash
# Try to delete category that has articles (should fail)
DELETE https://localhost:7067/odata/Category(1)
```

## 📈 Advanced Queries

### **1. Date Range Reports**
```bash
# Get articles from December 2024
GET https://localhost:7067/odata/NewsArticle?$filter=CreatedDate ge 2024-12-01T00:00:00Z and CreatedDate le 2024-12-31T23:59:59Z&$orderby=CreatedDate desc
```

### **2. Author Statistics**
```bash
# Get articles by specific author
GET https://localhost:7067/odata/NewsArticle?$filter=CreatedById eq 2&$expand=Category
```

### **3. Category Statistics**
```bash
# Get all categories with their articles
GET https://localhost:7067/odata/Category?$expand=NewsArticles&$select=CategoryId,CategoryName
```

## 🎯 OData vs Regular API Comparison

| Feature | Regular API | OData API |
|---------|-------------|-----------|
| **Filtering** | `?keyword=tech` | `?$filter=contains(CategoryName,'tech')` |
| **Sorting** | `?orderby=name` | `?$orderby=CategoryName` |
| **Pagination** | `?page=1&pageSize=10` | `?$skip=0&$top=10` |
| **Field Selection** | Not supported | `?$select=CategoryId,CategoryName` |
| **Expansion** | Not supported | `?$expand=ParentCategory` |
| **Counting** | Not supported | `?$count=true` |
| **Complex Queries** | Limited | Full OData query language |

## 🚨 Common Issues Fixed

### **1. Circular Reference Issue**
- **Problem**: `System.Text.Json.JsonException: A possible object cycle was detected`
- **Solution**: Added `ReferenceHandler.IgnoreCycles` in JSON configuration
- **Result**: No more circular reference errors

### **2. OData Query Support**
- **Before**: Limited filtering and sorting
- **After**: Full OData query language support
- **Benefits**: More powerful client-side querying

## 🔍 Testing Tools

### **1. Browser Testing**
- Open `https://localhost:7067/odata/$metadata` to see the data model
- Use browser for simple GET requests

### **2. Postman/Insomnia**
- Use for complex queries and POST/PUT/DELETE operations
- Set `Content-Type: application/json` for POST/PUT

### **3. curl Examples**
```bash
# Simple GET
curl "https://localhost:7067/odata/Category"

# With filter
curl "https://localhost:7067/odata/NewsArticle?\$filter=NewsStatus eq true"

# With multiple parameters
curl "https://localhost:7067/odata/NewsArticle?\$filter=NewsStatus eq true&\$orderby=CreatedDate desc&\$top=5"
```

## ✅ Success Indicators

1. **No Circular Reference Errors**: JSON serialization works without errors
2. **OData Queries Work**: All `$filter`, `$orderby`, `$select`, `$expand` work
3. **Business Rules Enforced**: Account/Category deletion rules still work
4. **Metadata Available**: `$metadata` endpoint returns schema
5. **CRUD Operations**: All Create, Read, Update, Delete operations work

## 🎉 Benefits of OData Implementation

1. **Powerful Querying**: Client can filter, sort, paginate without server changes
2. **Standardized API**: Follows OData protocol standards
3. **Rich Metadata**: Self-describing API with `$metadata`
4. **Flexible Expansion**: Load related data in single request
5. **Better Performance**: Client-side filtering reduces server load
6. **Developer Experience**: More intuitive query syntax

