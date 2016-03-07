<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">

  <xsl:variable name="SystemImagePath" select="/Root/Settings/@SystemImagePath"/>
  <xsl:variable name="PrintType" select="/Root/Settings/@PrintType"/>
  <xsl:variable name="QuestionStyle" select="/Root/Settings/@QuestionStyle"/>
  <xsl:variable name="AnswerStyle" select="/Root/Settings/@AnswerStyle"/>
  <xsl:variable name="HideTeacher" select="/Root/Settings/@HideTeacher"/>
  <xsl:variable name="HideStudent" select="/Root/Settings/@HideStudent"/>
  <xsl:variable name="HideDistrict" select="/Root/Settings/@HideDistrict"/>
  <xsl:variable name="HideBubbles" select="/Root/Settings/@HideBubbles"/>
  <xsl:variable name="true" select="1"/>
  <xsl:variable name="false" select="0"/>
  <xsl:variable name="ShowRubricOnAssessment" select="/Root/Settings/@ShowRubricOnAssessment"/>


  <xsl:output method="html"/>
  <xsl:param name="MCASpath"/>
  <xsl:param name="Inspectpath"/>
  <xsl:param name="NWEApath"/>

  <xsl:template match="/">
    <html>
      <head>
        <style type="text/css">
          .answer_first_cell_span {
          position:relative;
          top:3px;
          left:3px;
          margin-right:0px;
          }
        </style>
        <link rel="stylesheet" type="text/css" href="{$MCASpath}"/>
        <link rel="stylesheet" type="text/css" href="{$Inspectpath}"/>
        <link rel="stylesheet" type="text/css" href="{$NWEApath}"/>
      </head>
      <xsl:apply-templates select="*"/>
    </html>
  </xsl:template>

  <xsl:template match="Text">
  </xsl:template>


  <xsl:template match="Root">
    <xsl:apply-templates select="*"/>
  </xsl:template>

  <xsl:template match="Assessment">
    <xspan id="header">
      <div id="test_header" style="abcpdf-tag-visible: true;" >
        <div>
          <xsl:call-template name="new_hr"/>
          <table>
            <xsl:if test="$HideStudent = 'No'">
              <tr>
                <td nowrap="yes" style="padding-bottom:10px">Student Name:</td>
                <td nowrap="yes" colspan="3">______________________</td>
              </tr>
            </xsl:if>
            <xsl:if test="$HideTeacher = 'No'">
              <tr>
                <td nowrap="yes" style="padding-bottom:10px">Teacher:</td>
                <td nowrap="yes">______________________</td>
                <td>Date:</td>
                <td nowrap="yes">___________</td>
              </tr>
            </xsl:if>
            <xsl:if test="$HideDistrict = 'No'">
              <tr>
                <td  style="padding-bottom:10px">District:</td>
                <td nowrap="yes" colspan="3">
                  <xsl:value-of select="@District"/>
                </td>
              </tr>
            </xsl:if>
            <tr>
              <td  style="padding-bottom:10px">Assessment:</td>
              <td nowrap="yes" colspan="3"  style="padding-bottom:10px">
                <xsl:value-of select="@TestDesc"/>
              </td>
            </tr>
            <xsl:if test="@TestDescription != ''">
              <tr>
                <td  style="padding-bottom:10px">Description:</td>
                <td  style="padding-bottom:10px">
                  <xsl:value-of select="@TestDescription" disable-output-escaping="yes"/>
                </td>
              </tr>
            </xsl:if>
            <xsl:if test="@FormID != ''">
              <tr>
                <td  style="padding-bottom:10px">Form:</td>
                <td  style="padding-bottom:10px">
                  <xsl:value-of select="@FormID"/>
                </td>
              </tr>
            </xsl:if>
          </table>
        </div>
        <xsl:call-template name="new_hr"/>
        <br/>
        <xsl:if test="@AdministrationDirections!=''">
          <B>Administration Directions: </B>
          <xsl:value-of select="@AdministrationDirections" disable-output-escaping="yes"/>
          <br/>
        </xsl:if>
        <xsl:if test="@AdministrationDirections!='' and @Directions!=''">
          <br/>
        </xsl:if>
        <xsl:if test="@Directions!=''">
          <B>Student Directions: </B>
          <xsl:value-of select="@Directions" disable-output-escaping="yes"/>
        </xsl:if>
        <xsl:if test="@AdministrationDirections!='' or @Directions!=''">
          <xsl:call-template name="new_hr"/>
          <br/>
        </xsl:if>
      </div>
    </xspan>
    <xsl:choose>
      <xsl:when test="$PrintType = 'Rubric'">
        <xsl:apply-templates select="Rubrics"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:apply-templates select="*"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template match="Questions">
    <xsl:apply-templates select="*"/>
  </xsl:template>

  <xsl:template match="BankQuestion">
    <xsl:apply-templates select="*"/>
  </xsl:template>

  <xsl:template match="Question">
    <xsl:param name="RubricSection" select="$false"/>
    <DIV style="page-break-inside: avoid;">
      <xsl:if test="@Directions">
        <xsl:if test="@Sort &gt; 1 or name(..) = 'Addendum'">
          <xsl:call-template name="new_hr"/>
        </xsl:if>
        <B>
          Item <xsl:value-of select="@Sort"/> Directions:
        </B>
        <xsl:value-of select="@Directions" disable-output-escaping="yes"/>
        <xsl:call-template name="new_hr"/>
        <br/>
      </xsl:if>
      <TABLE style="{$QuestionStyle}">
        <TR>
          <TD nowrap="yes" valign="top" width="16">
            <B>
              <xsl:value-of select="@Sort"/>.
            </B>
          </TD>
          <TD>
            <xsl:value-of select="Text" disable-output-escaping="yes"/>
          </TD>
        </TR>
      </TABLE>
      <div style="margin-bottom: 3px;">
        <xsl:if test="$PrintType='AnswerKey' and @StandardName">
          <xsl:value-of select="@StandardName" disable-output-escaping="yes"/>
          <br/>
        </xsl:if>
        <xsl:if test="$PrintType='AnswerKey' and @Rigor">
          <xsl:value-of select="@Rigor" disable-output-escaping="yes"/>
          <br/>
        </xsl:if>
      </div>
      <xsl:if test="@ScoreType = 'R' and $PrintType = 'AnswerKey' and $RubricSection != $true">
        <div style="position:relative;top:3px;left:30px;margin-right:0px;">
          View rubric details in the rubric section at the end of the answer key
        </div>
      </xsl:if>
      <xsl:if test="$RubricSection = $true">
        <xsl:choose>
          <xsl:when test="/Root/Assessment/Rubrics/Rubric[@ItemNumber=current()/@Sort]">
            <xsl:value-of select="/Root/Assessment/Rubrics/Rubric[@ItemNumber=current()/@Sort]" disable-output-escaping="yes"/>
          </xsl:when>
          <xsl:otherwise>
            <span>
              <br/>
              (Although question is set up for a custom rubric, no custom rubric has been assigned to this item yet.)
              <br/>
            </span>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:if>
      <BR/>
      <xsl:apply-templates select="*"/>
    </DIV>
    <BR/> 
    
    <xsl:if test="@ScoreType='R' and $PrintType='Assessment' and $ShowRubricOnAssessment='Y' 
            and @DisplayRubric='Y' and $RubricSection!=$true ">
      <xsl:choose>
        <xsl:when test="/Root/Assessment/Rubrics/Rubric[@ItemNumber=current()/@Sort]">
          <xsl:variable name="RubricId" select="current()/@RubricID"/>
          <table style="border: solid 1px #585858;">
            <tr style="background-color:#585858; color: #fff; font-family: 'Times New Roman', 'Times', serif;  font-size: 1em;">
              <td>
                <xsl:choose>
                  <xsl:when test="$RubricId > 0">
                    Rubric <xsl:value-of select="$RubricId"/>
                  </xsl:when>
                  <xsl:otherwise>
                    Default Rubric - <xsl:value-of select="current()/@RubricPoints"/> point
                  </xsl:otherwise>
                </xsl:choose>
              </td>
            </tr>
            <tr>
              <td>
                <table style="cellpadding=2px; cellspacing:3px;">
                  <tr>
                    <td>
                      <xsl:value-of select="/Root/Assessment/Rubrics/Rubric[@ItemNumber=current()/@Sort]" disable-output-escaping="yes"/>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
          </table> 
        </xsl:when>
        <xsl:otherwise>
          <span>
            <br/>
            (Although question is set up for a custom rubric, no custom rubric has been assigned to this item yet.)
            <br/>
          </span>
        </xsl:otherwise>
      </xsl:choose>
      <BR/>
    </xsl:if>

  </xsl:template>


  <xsl:template match="Answers">
    <TABLE>
      <xsl:apply-templates select="*"/>
    </TABLE>
  </xsl:template>

  <xsl:template match="Answer">
    <!--<xsl:if test="string-length(Text)&gt;0">-->    
      <xsl:if test="string-length(Text)&gt;=0">
        <TR>
        <xsl:attribute name="style">
          <xsl:value-of select="$AnswerStyle"/>
          <xsl:if test="$PrintType='AnswerKey' and ../../@Correct = position()">
            ;background-color: yellow;
          </xsl:if>
        </xsl:attribute>
        <TD>
          <!--<span class="answer_first_cell">-->
          <xsl:choose>
            <xsl:when test="$HideBubbles = 'No'">
              <img src="{$SystemImagePath}\circle.gif" style="$imgStyle"/>
            </xsl:when>
            <xsl:otherwise>&#xA0;&#xA0;</xsl:otherwise>
          </xsl:choose>
          <!--</span>-->
        </TD>
        <TD>
          <xsl:value-of select="@Letter"/>
          <xsl:text disable-output-escaping="yes">.<![CDATA[&nbsp;&nbsp;&nbsp;]]></xsl:text>
        </TD>
        <TD>
          <xsl:value-of select="Text" disable-output-escaping="yes"/>
        </TD>
      </TR>
    </xsl:if>
  </xsl:template>

  <xsl:template name="new_hr">
    <xsl:choose>
      <xsl:when test="/Root/Assessment/@Proofed = 'true'">
        <hr/>
      </xsl:when>
      <xsl:otherwise>
        <table id="draft_hr" style="width: 100%">
          <tr>
            <td width="45%">
              <hr/>
            </td>
            <td width="1%">Draft</td>
            <td width="45%">
              <hr/>
            </td>
          </tr>
        </table>
      </xsl:otherwise>

    </xsl:choose>

  </xsl:template>

  <xsl:template match="Addendum">
    <pspan id="passage">
      <div id="passage" style="abcpdf-tag-visible: true;">
        <div>
          <xsl:if test="Question[1]/@Sort > 1">
            <xsl:attribute name="style">
              page-break-inside: always; <xsl:value-of select="$QuestionStyle"/>
            </xsl:attribute>
          </xsl:if>

          <xsl:text disable-output-escaping="yes"><![CDATA[&nbsp;]]></xsl:text>
          <!-- having this space here seems to make the group work better in some situations -->
          <xsl:choose>
            <xsl:when test="@Type='directions'"></xsl:when>
            <xsl:otherwise>
              <xsl:variable name="qCount" select="count(Question)"/>
              <xsl:if test="$qCount > 0">
                <br/>
                <b>
                  Please use the following <xsl:value-of select="@Type"/>
                  <xsl:choose>
                    <xsl:when test="$qCount = 1"> for this question.</xsl:when>
                    <xsl:otherwise>
                      for questions <xsl:value-of select="Question[1]/@Sort"/> through <xsl:value-of select="Question[last()]/@Sort"/>:
                    </xsl:otherwise>
                  </xsl:choose>
                </b>
                <br/>
                <br/>
              </xsl:if>
            </xsl:otherwise>
          </xsl:choose>
          <xsl:value-of select="Text" disable-output-escaping="yes"/>
        </div>
      </div>
    </pspan>
    <xsl:apply-templates select="*"/>
  </xsl:template>

  <xsl:template match="Rubrics">
    <xsl:if test="$PrintType = 'AnswerKey' or $PrintType = 'Rubric'">
      <pspan id="passage">
        <div id="passage" style="abcpdf-tag-visible: true;">
          <br/>
          <br/>
          <br/>
          <xsl:if test="count(/Root/Assessment/Questions/Question[@ScoreType='R']) &gt; 0 or count(/Root/Assessment/Questions/Addendum/Question[@ScoreType='R']) &gt; 0">
            <span style="{$QuestionStyle}; font-weight: bold;">Rubric Section</span>
            <xsl:apply-templates select="//Question[@ScoreType='R']">
              <xsl:with-param name="RubricSection" select="$true"/>
            </xsl:apply-templates>
            <!-- <xsl:apply-templates select="/Root/Assessment/Questions/Addendum/Question[@ScoreType='R']">
              <xsl:with-param name="RubricSection" select="$true"/>
            </xsl:apply-templates>-->
          </xsl:if>
        </div>
      </pspan>
    </xsl:if>
  </xsl:template>

  <xsl:template match="Rubric">
    <DIV>
      <br/>
      <!-- Print the item number if it exists as an attribute -->
      <xsl:if test="@ItemNumber">
        <span>
          <b>
            Item number <xsl:value-of select="@ItemNumber"/>
          </b>
        </span>
      </xsl:if>
      <div>
      </div>
      <xsl:value-of select="Text" disable-output-escaping="yes"/>
    </DIV>
  </xsl:template>

  <xsl:template match="Image">
    <DIV>
      <img src='{@Source}' alt='Print Image' />
    </DIV>
  </xsl:template>

  <xsl:template match="Standards">
    <TABLE>
      <xsl:apply-templates select="*"/>
    </TABLE>
  </xsl:template>

  <xsl:template match="Standard">
    <TR>
      <TD>
        <xsl:value-of select="StandardName" disable-output-escaping="yes"/>
      </TD>
      <TD>
      </TD>
      <TD>
        <xsl:value-of select="StandardText" disable-output-escaping="yes"/>
      </TD>
    </TR>
  </xsl:template>

  <xsl:template match="CoverPage">
    <xsl:value-of select="Text" disable-output-escaping="yes" />
  </xsl:template>


</xsl:stylesheet>
