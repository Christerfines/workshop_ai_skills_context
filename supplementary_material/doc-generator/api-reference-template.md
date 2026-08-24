# NordicBike Portal API Reference

## Overview

Describe the documented API area and its intended caller.

## Endpoint: `METHOD /path`

**Purpose**

State the observable operation.

**Authorization and Ownership**

State required role, identity source, and ownership rule. State `None` only when the source confirms it.

**Request**

| Location | Field | Type | Required | Rules |
| --- | --- | --- | --- | --- |
| Body, path, query, or header | Field name | Type | Yes or No | Validation rule |

```json
{
  "example": "request payload"
}
```

**Responses**

| Status | When returned | Body |
| --- | --- | --- |
| 200, 201, 204, 400, 404, or other observed status | Source-backed condition | Response shape or `None` |

```json
{
  "example": "success response"
}
```

**Notes**

Record observable side effects such as audit events, status transitions, or idempotency behaviour. Do not infer implementation details that are not visible in the source.