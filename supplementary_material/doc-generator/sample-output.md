# NordicBike Portal API Reference

## Product Catalog

Browse the active NordicBike product catalog. Product results can be filtered and paginated for the shop experience.

### List Products

`GET /api/products`

Returns products matching the optional search and catalog filters.

#### Authorization

No sign-in is required.

#### Query Parameters

| Name | Type | Required | Description |
| --- | --- | --- | --- |
| `q` | string | No | Matches product name, description, or tag. |
| `category` | string | No | Matches the product category. |
| `type` | string | No | Matches the product type. |
| `tag` | string | No | Matches an exact product tag. |
| `page` | integer | No | Page number. Defaults to `1`. |
| `pageSize` | integer | No | Items per page. Defaults to `24`; values are limited to `1` through `100`. |

#### Example Request

```http
GET /api/products?category=E-bikes&page=1&pageSize=12
```

#### Success Response

`200 OK`

```json
{
  "items": [
    {
      "id": "aurora-x3",
      "name": "Aurora X3",
      "category": "E-bikes",
      "price": 3499.00
    }
  ],
  "total": 1,
  "page": 1,
  "pageSize": 12
}
```

#### Notes

- Search and category filters are case-insensitive.
- Results are ordered by category, then product name.
- A request for a page outside the available result set returns `200 OK` with an empty `items` array.