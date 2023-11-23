/*
The type checker receives an asbtract syntax tree and checks that it
follows some rules.
The type checker doesnt create new elements, it just uses the constants
provided on the Element class to represent the objects.
*/

namespace Frontend;

class TypeChecker : IVisitorStmt<object?,Element>, IVisitorExpr<Element,Element>
{
    Scope<Element> globalScope = new Scope<Element>();
    //Analyze the semantic of the program to see if it is correct.
    public void Check(Program program)
    {
        foreach (Stmt stmt in program.Stmts)
        {
            Check(stmt);
        }
    }
    private void Check(Stmt stmt)
    {
        stmt.Accept(this, globalScope);
    }
    //Checking statements
    public object? VisitPointStmt(Stmt.Point pointStmt,Scope<Element> scope){
        if(scope.IsConstant(pointStmt.Id.Lexeme))throw new ExtendedException(pointStmt.Line,pointStmt.Offset,$"Redeclaration of constant {pointStmt.Id.Lexeme}");//Rule 1
        if(scope.HasBinding(pointStmt.Id.Lexeme))throw new ExtendedException(pointStmt.Line,pointStmt.Offset,$"Point `{pointStmt.Id.Lexeme}` is declared twice on the same scope");//Rule 3
        scope.SetArgument(pointStmt.Id.Lexeme,Element.POINT);
        return null;
    }
    public object? VisitConstantDeclarationStmt(Stmt.ConstantDeclaration declStmt,Scope<Element> scope){
        if(scope.IsConstant(declStmt.Id.Lexeme))throw new ExtendedException(declStmt.Line,declStmt.Offset,$"Redeclaration of constant {declStmt.Id.Lexeme}");//Rule 1
        Element rValue = Check(declStmt.Rvalue,scope);
        scope.SetConstant(declStmt.Id.Lexeme,rValue);
        return null;
    }
    public object? VisitPrintStmt(Stmt.Print printStmt,Scope<Element> scope){
        Check(printStmt._Expr,scope);
        return null;
    }
    public object? VisitColorStmt(Stmt.Color colorStmt,Scope<Element> scope){
        //A color statement doesnt invloves any checking.
        return null;
    }
    public object? VisitDrawStmt(Stmt.Draw drawStmt,Scope<Element> scope)
    {
        Element element = Check(drawStmt._Expr,scope);
        //element is of class Element, but its an instance of a subclass of Element, some of which implements the interface IDrawable. Rule # 5
        if(!(element is IDrawable))throw new ExtendedException(drawStmt._Expr.Line,drawStmt._Expr.Offset,$"Element of type `{element.Type}` is not drawable");
        return null;
    }
    //Checking expressions
    private Element Check(Expr expr,Scope<Element> scope){
        return expr.Accept(this,scope);
    }
    public Element VisitNumberExpr(Expr.Number numberExpr,Scope<Element> scope){
        return Element.NUMBER;
    }
    public Element VisitStringExpr(Expr.String stringExpr,Scope<Element> scope){
        return Element.STRING;
    }
    public Element VisitVariableExpr(Expr.Variable variableExpr,Scope<Element> scope){
        if(!scope.HasBinding(variableExpr.Id.Lexeme,true))throw new ExtendedException(variableExpr.Line,variableExpr.Offset,$"Variable `{variableExpr.Id.Lexeme}` used but not declared");//Rule 4
        return scope.Get(variableExpr.Id.Lexeme);
    }
    public Element VisitUnaryNotExpr(Expr.Unary.Not unaryNotExpr, Scope<Element> scope){
        Check(unaryNotExpr._Expr,scope);//Check the right hand expr.
        return Element.NUMBER;//Rule # 6
    }
    public Element VisitUnaryMinusExpr(Expr.Unary.Minus unaryMinusExpr, Scope<Element> scope){
        //Check the right hand expr.
        //Rule # 7
        Element rValue = Check(unaryMinusExpr._Expr,scope);
        if(rValue.Type != ElementType.NUMBER)throw new ExtendedException(unaryMinusExpr.Line,unaryMinusExpr.Offset,$"Applied `-` operator to a {rValue.Type} operand");
        return Element.NUMBER;
    }
    public Element VisitBinaryPowerExpr(Expr.Binary.Power powerExpr, Scope<Element> scope){
        CheckNumberOperands(powerExpr,scope);
        return Element.NUMBER;
    }
    public Element VisitBinaryProductExpr(Expr.Binary.Product productExpr, Scope<Element> scope){
        CheckNumberOperands(productExpr,scope);
        return Element.NUMBER;
    }
    public Element VisitBinaryDivisionExpr(Expr.Binary.Division divisionExpr, Scope<Element> scope){
        CheckNumberOperands(divisionExpr,scope);
        return Element.NUMBER;
    }
    public Element VisitBinaryModulusExpr(Expr.Binary.Modulus modulusExpr, Scope<Element> scope){
        CheckNumberOperands(modulusExpr,scope);
        return Element.NUMBER;
    }
    public Element VisitBinarySumExpr(Expr.Binary.Sum sumExpr, Scope<Element> scope){
        CheckNumberOperands(sumExpr,scope);
        return Element.NUMBER;
    }
    public Element VisitBinaryDifferenceExpr(Expr.Binary.Difference differenceExpr, Scope<Element> scope){
        CheckNumberOperands(differenceExpr,scope);
        return Element.NUMBER;
    }
    public Element VisitBinaryLessExpr(Expr.Binary.Less lessExpr, Scope<Element> scope){
        CheckNumberOperands(lessExpr,scope);
        return Element.NUMBER;
    }
    public Element VisitBinaryLessEqualExpr(Expr.Binary.LessEqual lessEqualExpr, Scope<Element> scope){
        CheckNumberOperands(lessEqualExpr,scope);
        return Element.NUMBER;
    }
    public Element VisitBinaryGreaterExpr(Expr.Binary.Greater greaterExpr, Scope<Element> scope){
        CheckNumberOperands(greaterExpr,scope);
        return Element.NUMBER;
    }
    public Element VisitBinaryGreaterEqualExpr(Expr.Binary.GreaterEqual greaterEqualExpr, Scope<Element> scope){
        CheckNumberOperands(greaterEqualExpr,scope);
        return Element.NUMBER;
    }
    private void CheckNumberOperands(Expr.Binary binaryExpr, Scope<Element> scope){
        Element operand = Check(binaryExpr.Left,scope);//Check left operand
        if(operand.Type != ElementType.NUMBER)throw new ExtendedException(binaryExpr.Left.Line,binaryExpr.Left.Offset,$"Left operand of `{binaryExpr.Operator.Lexeme}` is {operand.Type} and must be NUMBER");
        operand = Check(binaryExpr.Right,scope);//Check right operand
        if(operand.Type != ElementType.NUMBER)throw new ExtendedException(binaryExpr.Right.Line,binaryExpr.Right.Offset,$"Right operand of `{binaryExpr.Operator.Lexeme}` is {operand.Type} and must be NUMBER");
    }
    public Element VisitBinaryEqualEqualExpr(Expr.Binary.EqualEqual equalEqualExpr, Scope<Element> scope){
        Check(equalEqualExpr.Left,scope);
        Check(equalEqualExpr.Right,scope);
        //Equality doesnt present any semantic problems.
        return Element.NUMBER;
    }
    public Element VisitBinaryNotEqualExpr(Expr.Binary.NotEqual notEqualExpr, Scope<Element> scope){
        Check(notEqualExpr.Left,scope);
        Check(notEqualExpr.Right,scope);
        //Equality doesnt present any semantic problems.
        return Element.NUMBER;
    }
    public Element VisitBinaryAndExpr(Expr.Binary.And andExpr, Scope<Element> scope){
        Check(andExpr.Left,scope);
        Check(andExpr.Right,scope);
        //Logical and doesnt present any semantic problems.
        return Element.NUMBER;
    }
    public Element VisitBinaryOrExpr(Expr.Binary.Or orExpr, Scope<Element> scope){
        Check(orExpr.Left,scope);
        Check(orExpr.Right,scope);
        //Logical or doesnt present any semantic problems.
        return Element.NUMBER;
    }
    public Element VisitConditionalExpr(Expr.Conditional conditionalExpr, Scope<Element> scope){
        Check(conditionalExpr.Condition,scope);//Check condition
        Element thenBranchElement = Check(conditionalExpr.ThenBranchExpr,scope);//Check if branch expression
        Element elseBranchElement = Check(conditionalExpr.ElseBranchExpr,scope);//Check else branch expression
        if(thenBranchElement.Type != elseBranchElement.Type)throw new ExtendedException(conditionalExpr.Line,conditionalExpr.Offset,$"Expected equal return types for `if-then-else` expression branches, but {thenBranchElement.Type} and {elseBranchElement.Type} were found.");
        return thenBranchElement;
    }
}