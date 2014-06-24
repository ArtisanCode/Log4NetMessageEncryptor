Feature: LogEncryption
	In order to secure log messages at rest
	As a security/compliance officer
	I want to encrypt the output from log4net
	
Scenario: Encrypt Log information
	Given Log4Net is configured
	When I log a test message
	Then there should be one log message present
	And  the result should be encrypted

