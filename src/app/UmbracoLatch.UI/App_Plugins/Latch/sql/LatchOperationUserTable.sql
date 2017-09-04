CREATE TABLE LatchOperationUser(
	UserId INT NOT NULL,
	LatchOperationId INT NOT NULL,
	
	CONSTRAINT FK_LatchOperationUser_UmbracoUser FOREIGN KEY(UserId)
    REFERENCES umbracoUser (Id)
    ON DELETE CASCADE,
    
    CONSTRAINT FK_LatchOperationUser_LatchOperation FOREIGN KEY(LatchOperationId)
    REFERENCES LatchOperation (Id)
)