#Project Description#
**Note this project is not maintained anymore. It is targeted  at .NET Micro Framework v3.0 but with some modifications it should be possible
to use it for .NET Compact and desktop frameworks.**
Welcome at the MFBasic project page. MFBasic is a basic interpetter written in C#. It is targeted  at .NET Micro Framework but with some modifications it should be possible
to use it for .NET Compact and desktop frameworks.

It's based on uBasic ([url:http://www.sics.se/~adam/ubasic/]) but it's completely rewritten to be more modular and support more features.

Some of the supported features:
* WHILE -> WEND
* IF, ELSE, ENDIF
* FOR -> NEXT, STEP
* Line numbers of QBasic like labels
* Custom variable names (Variables ending with $ are string variables others are considered floating point)
* Easy to create custom functions
* Basic sourcecode is precompiled to run faster

MFBasic is in alpha state right now.. I'm currently working on a State storage to store and restore current running state. Also I will add more functions and hopefully implement arrays

##Demo application##
    ' This is the start of the demo application
    ' This program showcases the use of MFBasic

            ' Sample string variable
            text$ = "Hallo world!"
            
            ' Move to sub
            GOSUB sub
            
            ' Call routines and print output of numeric function
            RANDOMIZE
            PRINT INT(RND()*1000*10)
                    
            ' Create new floating point variable from function result
            fnumber = CINT(9.8)
            
            ' For, Next loop
            FOR i = 1 TO 10
                IF i <= 5 THEN 
                    GOSUB sub2 
                ELSE 
                    PRINT i*fnumber
                ENDIF
            NEXT i
            
            ' Text should be aaaa else a program error occourd
            IF (text$="aaaaa") THEN PRINT "Ok!"
            
            ' End of this demo program
            PRINT "End Of Program"
            PRINT text$
            
            END
            
            ' Subroutine 1
            ' Clear text variable
            
    sub:    PRINT "Subroutine"
            text$ = ""
            RETURN
            
            
            ' Subroutine 2
            ' Add a to text variable
            
    sub2:   PRINT "Five"
            text$ = text$ + "a"
            RETURN    
