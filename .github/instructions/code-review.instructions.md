---
description: 'Code review instructions for GitHub Copilot'
applyTo: '**'
excludeAgent: ["coding-agent"]
---

## General Review Principles

When performing a code review, follow these principles:

1. **Be specific**: Reference exact lines, files, and provide concrete examples
2. **Provide context**: Explain WHY something is an issue and the potential impact
3. **Suggest solutions**: Show corrected code when applicable, not just what's wrong
4. **Be constructive**: Focus on improving the code, not criticizing the author
5. **Recognize good practices**: Acknowledge well-written code and smart solutions
6. **Be pragmatic**: Not every suggestion needs immediate implementation
7. **Group related comments**: Avoid multiple comments about the same topic

## Code Quality Standards

When performing a code review, check for:

### Clean Code
- Descriptive and meaningful names for variables, methods, and classes
- Single Responsibility Principle: each function/class does one thing well
- DRY (Don't Repeat Yourself): no code duplication
- Avoid deeply nested code (max 3-4 levels)
- Avoid magic numbers and strings (use constants)
- Code should be self-documenting; comments only when necessary

### Error Handling
- Proper error handling at appropriate levels
- Meaningful error messages
- No silent failures or ignored exceptions
- Fail fast: validate inputs early
- Use appropriate error types/exceptions

## Security Review

When performing a code review, check for security issues:

- **Sensitive Data**: No passwords, API keys, tokens, or PII in code or logs
- **Input Validation**: All user inputs are validated and sanitized
- **SQL Injection**: Use parameterized queries, never string concatenation
- **Authentication**: Proper authentication checks before accessing resources
- **Authorization**: Verify user has permission to perform action
- **Cryptography**: Use established libraries, never roll your own crypto
- **Dependency Security**: Check for known vulnerabilities in dependencies

## Testing Standards

When performing a code review, verify test quality:

- **Coverage**: Critical paths and new functionality must have tests
- **Test Names**: Descriptive names that explain what is being tested
- **Test Structure**: Clear Arrange-Act-Assert or Given-When-Then pattern
- **Independence**: Tests should not depend on each other or external state
- **Assertions**: Use specific assertions, avoid generic assertTrue/assertFalse
- **Edge Cases**: Test boundary conditions, null values, empty collections
- **Mock Appropriately**: Mock external dependencies, not domain logic

## Architecture and Design

When performing a code review, verify architectural principles:

- **Separation of Concerns**: Clear boundaries between layers/modules
- **Dependency Direction**: High-level modules don't depend on low-level details
- **Interface Segregation**: Prefer small, focused interfaces
- **Loose Coupling**: Components should be independently testable
- **High Cohesion**: Related functionality grouped together
- **Consistent Patterns**: Follow established patterns in the codebase

## Documentation Standards

When performing a code review, check documentation:

- **Complex Logic**: Non-obvious logic should have explanatory comments
- **README Updates**: Update README when adding features or changing setup
- **Breaking Changes**: Document any breaking changes clearly
- **Examples**: Provide usage examples for complex features

## Comment Format Template

When performing a code review, use this format for comments:

```markdown
**[PRIORITY] Category: Brief title**

Detailed description of the issue or suggestion.

**Why this matters:**
Explanation of the impact or reason for the suggestion.

**Suggested fix:**
[code example if applicable]

**Reference:** [link to relevant documentation or standard]
```

## Review Checklist

When performing a code review, systematically verify:

### Code Quality
- [ ] Code follows consistent style and conventions
- [ ] Names are descriptive and follow naming conventions
- [ ] Functions/methods are small and focused
- [ ] No code duplication
- [ ] Complex logic is broken into simpler parts
- [ ] Error handling is appropriate
- [ ] No commented-out code or TODO without tickets

### Security
- [ ] No sensitive data in code or logs
- [ ] Input validation on all user inputs
- [ ] No SQL injection vulnerabilities
- [ ] Authentication and authorization properly implemented
- [ ] Dependencies are up-to-date and secure

### Testing
- [ ] New code has appropriate test coverage
- [ ] Tests are well-named and focused
- [ ] Tests cover edge cases and error scenarios
- [ ] Tests are independent and deterministic
- [ ] No tests that always pass or are commented out

### Architecture
- [ ] Follows established patterns and conventions
- [ ] Proper separation of concerns
- [ ] No architectural violations
- [ ] Dependencies flow in correct direction

### Documentation
- [ ] Complex logic has explanatory comments
- [ ] README is updated if needed
- [ ] Breaking changes are documented