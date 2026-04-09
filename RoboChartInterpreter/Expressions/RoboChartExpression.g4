grammar RoboChartExpression;

expression:
	literal												# LiteralExpr
	| NAME												# VariableExpr
	| left = expression op = ('==' | '!=') expression	# BinaryExpr;

literal:
	INT			# IntLiteral
	| STRING	# StringLiteral
	| REAL		# RealLiteral;

INT: [0-9]+;
REAL: [0-9]+ '.' [0-9]*;
NAME: [a-zA-Z_][a-zA-Z_0-9]*;
STRING: '"' ~[\r\n"]* '"';
WS: [ \t\n\r\f]+ -> skip;