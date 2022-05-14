# Security Policy

## Supported Versions

Following versions are supported with security updates.

| Version | Supported          |
| ------- | ------------------ |
| < 1.0   | :white_check_mark: |

## Known vulnerabilities

No known vulnerabilities.

## Steps of the Vulnerability Management

1. Discover: The person discovering the issues (the reporter) **privately** reports it to [webmaster@wgnf.de](mailto:webmaster@wgnf.de?subject=liz:%20Vulnerability%20Report) providing all the information needed to resolve the problem - ideally already providing the severity of the issue (choose either from a blanket severity of low, medium, high, critical or use the [CVSS Calculator](https://www.first.org/cvss/calculator/3.0#CVSS:3.0/AV:N/AC:L/PR:H/UI:N/S:U/C:N/I:N/A:N))
2. Acknowledge: The owner of liz will reply in the following business days to acknowledge the receipt
3. Assess: If not already done the severity of the vulnerability is determined (see above)
4. Investigate: The reported vulnerability will be investigated. The reporter will receive a reply if the vulnerability has been accepted or rejected (if rejected the process stops here)
5. Remediate: We will patch the vulnerability ourselves or forward the report to the affected packages/systems (in the case of multiple open vulnerabilities the riskiest according to the severity or CVSS score are prioritized). The potential fix and announcement will be shared with the reporter
6. Verify: We will make sure that the potential fix is actually fixing the vulnerability (using additional scans and/or tests) in cooperation with the reporter
7. Disclosure: If the planned fix is accepted it will be published alongside an announcement and a new (patch) release, as well as adding it to the list above

Any person involved in this process should keep the reported vulnerability confidential until it has been fixed. This means avoiding public GitHub issues or commits.
