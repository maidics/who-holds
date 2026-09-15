# ADR

## Instructions

Always be as precise as possible with the wording while reducing the size of the document. Long paragraphs restating what has already been written are not needed. For repeated elements use linking and reference previously stated information.

Write concise text regarding the decision and mechanics chosen while weighing pros and cons:
- Open with the decision in one sentence. Name the mechanism, not the motivation.
- Name mechanics concretely: the API, call sequence, or library a reader needs to find it in the code. Prefer exact identifiers over descriptions.
- Give each rejected option one table and a one-line pros/cons pair. Drop options that were never realistic.
- Reduce the trade-off to a single axis, name the winning side, and say what it cost.
- Drop the options that were never taken into consideration.
- Mark the options that were not evaluated as unevaluated instead of rejected.
- Close with consequences: what gets easier, what gets harder, what triggers a revisit.
- Cut any sentence that restates the title or the context.

**No**: Use the documented Windows API for detecting which process holds a file.
**Yes**: Use [`RmStartSession`](https://learn.microsoft.com/en-us/windows/win32/api/restartmanager/nf-restartmanager-rmstartsession) → [`RmRegisterResources`](https://learn.microsoft.com/en-us/windows/win32/api/restartmanager/nf-restartmanager-rmregisterresources) → [`RmGetList`](https://learn.microsoft.com/en-us/windows/win32/api/restartmanager/nf-restartmanager-rmgetlist).

**No**: This approach was chosen because the previous one had several problems that made it unsuitable for continued use.
**Yes**: See [Context](#context) for failures.

**No**: The options trade off against each other across several dimensions.
**Yes**: Axis: supportability against coverage. Supportability wins. Cost: directories are uncovered.

## Tone

Write in impersonal, imperative voice. No "we", "I", or "the team".

- State decisions as imperatives: "Replace handle enumeration with the Restart Manager."
  Not: "We decided to replace..." / "I'm going to replace..."
- Describe the system in present indicative, active voice: "The lock-detection path enumerates every handle."
  Not: "Handle enumeration was chosen." (passive hides the agent without gaining objectivity)
- Attribute constraints to the system, not to people: "Full results require SeDebugPrivilege."
  Not: "We can't see everything unless we're elevated."
- Consequences describe effects, not feelings: "Empty results become ambiguous."
  Not: "We lose confidence in empty results."
- Titles take the same imperative: "Use the Restart Manager for file-lock detection."
  Not: "Switching to the Restart Manager" / "Why the Restart Manager?"

Facts about the author's knowledge rather than the system go in a Notes section at the end, where first person is allowed.

## Output format

Always refrain from generating text that reads as AI slop:

- Instead of using "—" characters use "-".
- Never use phrases such as "than it might first look", "turns out to be worth less than it looks".
- Never use "not X, but Y" framing.
- Never open a sentence with a filler qualifier.

**No**: This is not just a correctness fix, but a maintenance one.
**Yes**: The change removes a maintenance burden.

**No**: It's worth noting that sessions are capped at 64 per user session.
**Yes**: Sessions are capped at 64 per user session.

### Links

Always link official documentation for anything referenced by the document. Important: link an identifier on all mentions.

**No**: [`RmGetList`](url) returns the lockers. Call `RmGetList` again on `ERROR_MORE_DATA`.

**Yes**: [`RmGetList`](url) returns the lockers. [...] Call [`RmGetList`](url) again on `ERROR_MORE_DATA`.


### Tables

One clause per cell. No prose, no trailing punctuation.

**No**: | Complexity | Fairly high, since it needs version-conditional structs. |
**Yes**: | Complexity | High - version-conditional structs |

### Bullet points

Bullet points should always start with a capital letter and end with a punctuation mark.

**No**:
- do not use this
- Do not use this
- do not use this.
- do not use this?
- do not use this!

**Yes**:
- Use this.
- Use this!
- Use this?