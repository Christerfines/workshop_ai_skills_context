# Local change evidence

Range: v1.4.0..HEAD
Target revision: 789abcd

## Commits

- abc1234 feat: add saved carts
- def5678 fix: prevent duplicate checkout submission
- 789abcd docs: clarify local setup
- 456efgh refactor: simplify internal request mapper

## Diff summary

- `src/CartStore.cs`: adds saved-cart persistence for signed-in customers.
- `src/CheckoutService.cs`: prevents a repeated submit from creating a second order.
- `src/README.md`: adds local setup instructions.
- `src/InternalMapper.cs`: renames private helpers without changing the public contract.

This is a local evidence snapshot. It does not prove adoption, customer volume, or production rollout.
