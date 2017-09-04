CREATE TABLE LatchOperationNode(
	NodeId INT NOT NULL,
	LatchOperationId INT NOT NULL,
	
	CONSTRAINT FK_LatchOperationNode_UmbracoNode FOREIGN KEY(NodeId)
    REFERENCES umbracoNode (Id)
    ON DELETE CASCADE,
    
    CONSTRAINT FK_LatchOperationNode_LatchOperation FOREIGN KEY(LatchOperationId)
    REFERENCES LatchOperation (Id)
)