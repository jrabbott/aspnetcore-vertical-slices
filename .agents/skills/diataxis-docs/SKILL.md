---
name: diataxis-docs
description: Write or reorganise repository docs using the Diátaxis framework (tutorials, how-to, reference, explanation). Use when adding documentation, changing docs structure, or when the user mentions Diátaxis, tutorials, how-to guides, reference, or explanation.
license: MIT
metadata:
  author: weatherapp
  version: "1.0"
---

# Diátaxis documentation

Keep docs organised by reader need. Index: [docs/README.md](../../../docs/README.md).

When choosing a quadrant or naming a page, load [references/quadrant-guide.md](references/quadrant-guide.md).

## Workflow

1. Decide the need (learn / do a task / look up / understand).
2. Add or edit a file under the matching folder:
   - `docs/tutorials/`
   - `docs/how-to/`
   - `docs/reference/`
   - `docs/explanation/`
3. Update the tables in [docs/README.md](../../../docs/README.md) and the Diátaxis section of the root [README.md](../../../README.md) when adding a page.
4. Cross-link other quadrants instead of copying long explanations into how-tos (and vice versa).
5. Use **UK spelling** in prose and examples.
6. Contribution workflow (prerequisites, CI-parity, PRs) lives in [CONTRIBUTING.md](../../../CONTRIBUTING.md) — link it; do not duplicate the process here.

## Principles

- Tutorials teach a path; how-tos achieve a goal; reference states facts; explanation gives context.
- Prefer one purpose per page.
- Point agents and humans at existing docs rather than duplicating them inside skills or READMEs.

## Related

- [Contributing](../../../CONTRIBUTING.md)
- [Repository layout](../../../docs/reference/repository-layout.md)
- [Design choices](../../../docs/explanation/design-choices.md)
