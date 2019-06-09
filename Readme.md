## HttpValuesMutator

This is a work in progress. Please check back later to see how things are going!

Tasks:

- Only mutate when action has specific attribute
- Get overrides for property names in attribute, along with overrides for a function
  - Allow override attribute to override property name based on name of object it's part of (for the case of nested objects)
- Recursively search an object and account for different types, like `JArray` and `JValue`
- Pass the configuration into the filter at registration
- Cache reflection results
