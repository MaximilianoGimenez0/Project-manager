# Product

<!-- impeccable:product-schema 1 -->

## Platform

web

## Users

- **Proposal Submitters:** Team members and department leads who draft, estimate (budget, duration, impact area), and submit new project proposals for institutional or organizational backing.
- **Approvers and Managers:** Designated decision-makers and organizational leaders tasked with reviewing submitted proposals, assessing feasibility, verifying budget constraints, and executing step-by-step approval or rejection with documented rationale.

## Product Purpose

Automate and centralize the lifecycle of project proposals within an organization. Success means eliminating chaotic email/spreadsheet review chains by providing a single, auditable flow where proposals are submitted with required metadata and routed through structured approval stages to definitive decisions.

## Positioning

Structured multi-step approval governance with transparent audit trails, role-based evaluations, and clear budget/impact criteria tailored for internal organizational accountability.

## Operating Context

- Desktop and tablet web environments in professional/enterprise workplaces.
- Day-to-day interactions involve authoring structured proposals, filtering pending project lists, inspecting proposal details against criteria, and submitting step decisions through modal/dialog confirmation.
- Interacts with a .NET 8 Web API backend using CQRS and PostgreSQL persistence.

## Capabilities and Constraints

- **Capabilities:**
  - Proposal creation with title, description, estimated budget, estimated duration, and impact area.
  - Multi-step approval system with status progression and decision capture (approval/rejection with reviewer comments).
  - Search and filter portal to browse proposals by status, keyword, and project parameters.
  - Native internationalization (i18n) support (English and Spanish).
- **Technical Constraints:**
  - Built with React 19, TypeScript, Vite, React Router DOM, and Firebase Hosting.
  - Strict UI development guidelines: Vanilla CSS Modules per component, mobile-first responsive design, camelCase class naming, no utility CSS frameworks (e.g. Tailwind).
  - Architecture enforces separation of concerns: custom hooks handle state and network communication; components focus strictly on presentation.

## Brand Commitments

- **Name:** Project Manager & Approval System (`Project Manager`).
- **Voice & Personality:** Professional, direct, reliable, and clean internal B2B/SaaS aesthetic.
- **Visual Identity:** Soft SaaS slate/blue color palette, clean typography (`Inter`), structured cards, clear status badges.

## Evidence on Hand

- Repository source code under `src/` (components, pages, routes, hooks, services, i18n).
- Architecture and schema documentation in [README.md](file:///d:/Projects/ProjectProposals_frontend_react/README.md).
- Strict development specifications in [AGENTS.md](file:///d:/Projects/ProjectProposals_frontend_react/AGENTS.md).
- Live backend API deployed at `https://projectproposals-backend-1.onrender.com/api`.
- *Absence note:* No external marketing testimonials, client logos, or fabricated case studies exist; future work must not fabricate them.

## Product Principles

1. **Governance & Auditability:** Every decision, state transition, and reviewer remark is explicit, auditable, and transparent.
2. **Operational Efficiency:** Streamline data entry for submitters and provide high-density, easily scannable information for evaluators to minimize decision latency.
3. **Architectural Integrity:** Maintain strict separation between presentation and state logic, keeping component boundaries clean and predictable.
4. **Clarity Over Decoration:** Prioritize clear visual hierarchy, legible typography, distinct status indicators, and responsive utility over gratuitous ornament.
