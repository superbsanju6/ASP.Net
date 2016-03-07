namespace Thinkgate.Enums
{
    public enum LoginErrorCode
    {
        // Error Codes 0-999 are reserved for errors related to receipt of incorrect SSO claim information
        MissingClaimLeaIdentifier = 1,     // The LEA Identifier claim is missing
        MissingClaimTeacherIdentifier = 2, // The Teacher Identifier claim is missing
        EmptyClaimLeaIdentifier = 3,       // The LEA Identifer claim value is empty
        EmptyClaimTeacherIdentifier = 4,   // The Teacher Identifier claim value is empty
        NotFoundLeaIdentifier = 5,         // The LEA Identifier claim value does not map to an LEA
        NotFoundTeacherIdentifier = 6,     // The Teacher Identifer claim value does not map to a user
        // Error Codes 1000... are reserved for internal Thinkgate errors
        UnspecifiedError = 1000, // Unspecified exception occurred
    }
}