package model.rider

import com.jetbrains.rider.model.nova.ide.SolutionModel
import com.jetbrains.rd.generator.nova.*
import com.jetbrains.rd.generator.nova.PredefinedType.*
import com.jetbrains.rd.generator.nova.csharp.CSharp50Generator
import com.jetbrains.rd.generator.nova.kotlin.Kotlin11Generator

@Suppress("unused")
object VerifyModel : Ext(SolutionModel.Solution) {
    init {
        setting(CSharp50Generator.Namespace, "ReSharperPlugin.Verify")
        setting(Kotlin11Generator.Namespace, "com.jetbrains.rider.plugins.verify")

        signal("compare", structdef("CompareData") {
            field("testName", string)
            field("receivedFile", string)
            field("verifiedFile", string)
        })
    }
}